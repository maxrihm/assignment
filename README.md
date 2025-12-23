# User Authentication Stub

This repo is the assignment. Minimal login flow. .NET 8 API plus Next.js UI.

## Part 1 Architecture explanation

1. Backend architecture (C# API)

File structure. I keep it small but separated by responsibility.

* Controllers: AuthController, HTTP boundary only
* Dto: LoginRequest and LoginResponse, shapes that cross the wire
* Services: AuthService (login orchestration), AuthValidator (credential check), TokenService (token issuing)
* Interfaces: IAuthService, IAuthValidator, ITokenService
* Models: LoginResult (success vs invalid)
* Extensions: map LoginResult to HTTP responses

Login endpoint flow (request -> validation -> response).

* POST /api/auth/login receives JSON
* ASP.NET binds JSON to LoginRequest and validates required fields
* Controller calls AuthService.LoginAsync
* AuthService asks AuthValidator if username/password match expected values
* If valid, AuthService asks TokenService for a token string
* AuthService returns LoginResult.Success(token) or LoginResult.InvalidCredentials
* Controller converts result to HTTP

  * 200 with { token }
  * 401 with ProblemDetails

Responsibilities.

* Controller: HTTP, model binding, return ActionResult
* AuthService: business flow, no HTTP logic
* AuthValidator: only credential verification
* TokenService: only token creation/return (hardcoded in this assignment)
* Result mapping: keeps status codes and response bodies consistent

2. Frontend architecture (Next.js)

Login page organization.

* pages/login.tsx: page entry, renders LoginForm
* features/auth/components/LoginForm.tsx: form UI and state
* features/auth/api.ts: login(req) wrapper
* lib/http/client.ts: shared fetch wrapper and error mapping

API request/response handling.

* LoginForm calls login({ username, password })
* login uses apiPost to send POST to /api/auth/login
* apiPost

  * builds the full URL using API_BASE_URL
  * sends JSON with correct headers
  * reads JSON response
  * if response is not ok, tries to read ProblemDetails and converts it into a readable message
  * throws a ResponseError so the UI can show a message

Loading, error, success states.

* State machine in LoginForm

  * idle: form visible, button enabled only when inputs are filled
  * loading: button and inputs disabled, show Loading
  * success: show Success and token
  * error: show Error and message
* On any input change after an error, clear the error so the user is not stuck

3. Data flow

What happens when the user clicks Login.

* User submits the form
* UI switches to loading
* Frontend sends POST to the API with username/password
* Backend validates and returns 200 with token or 401 with ProblemDetails
* Frontend reads response

  * success: show token
  * failure: show error message

Where state lives and token handling.

* All state lives in the LoginForm component (username, password, status)
* Token is stored only in component state and only displayed
* No persistence (no localStorage/cookies) because the assignment only needs to display the token

4. Tradeoffs

**Backend (production)**

* Credentials: store users in a database and hash passwords using a proper password hashing scheme (Argon2id / BCrypt / PBKDF2 via ASP.NET Identity). Avoid plain SHA for password storage.
* AuthValidator / credential comparison: in this assignment, AuthValidator uses CryptographicOperations.FixedTimeEquals (constant-time compare). Other common options depend on the credential type: for passwords, use a password hashing algorithm (Argon2id, BCrypt, PBKDF2); for API keys/shared secrets, store a SHA-256 or HMAC-SHA256 hash and compare fixed-length bytes in constant time.
* Tokens: issue real JWTs with expiry/claims (and possibly refresh tokens) and add audit logging around login events.
* Brute-force protection (if the page is public): rate limiting / throttling, temporary lockout, and monitoring/alerts. For public-facing login, also consider IP/device signals and CAPTCHA after suspicious behavior.
* Validation: validate requests at the boundary (before the service layer) with a consistent approach (built-in model validation + optional FluentValidation)
* Response contract: in our production apps we typically return a consistent “response envelope” / custom result type (plus ProblemDetails mapping) so controllers stay thin and responses are uniform. I omitted that here because it adds a fair amount of shared plumbing for the assignment.
* Error handling: in production we usually have a custom exception-handling middleware that converts domain/infra exceptions into the correct HTTP + error body. For this assignment I kept it simple and used the standard ASP.NET Core exception handling instead
* Documentation: add Swagger/OpenAPI annotations (e.g., ProducesResponseType + example responses) so it’s obvious what 200 vs 401 look like.
* Environments: include stack traces only in Development, and avoid leaking internal details in Production (while still logging full context server-side).
* Testing (this login feature): 10 tests total (5 unit, 5 integration).
* Unit tests (5, xUnit): AuthService.LoginAsync valid returns Success and calls ITokenService; invalid returns InvalidCredentials and does not call ITokenService. AuthValidator.IsValidCredentials exact match is true; username mismatch is false; password mismatch is false.
* Integration tests (5, WebApplicationFactory): POST /api/auth/login returns 200 with JSON { token: "hardcoded-token" } for valid creds; returns 401 with ProblemDetails title Unauthorized and detail Invalid username or password for invalid creds; returns 400 with validation errors for missing username; returns 400 with validation errors for missing password; CORS preflight from an allowed origin returns expected CORS headers.

**Frontend (production)**

* Token handling: this assignment does not store tokens in localStorage; in production I would still avoid localStorage. Prefer httpOnly + Secure cookies (session/JWT) with a CSRF strategy, or use a BFF pattern. If tokens must be handled client-side, prefer in-memory storage and refresh/rotation (not localStorage).
* Network robustness: I would add request timeouts using AbortController (abort signal) so the UI doesn’t hang forever if the API becomes unresponsive
* Testing (this login feature): 12 tests total (7 component, 3 HTTP-client, 2 E2E).
* Component tests (7, React Testing Library): button disabled until username and password are non-empty (username trimmed); submit switches button text to “Logging in...” and disables inputs; success renders “Success! Token: ...”; 401 renders “Error: Invalid username or password.”; network failure renders “Error: Network error”; after an error, changing either input clears the error; aria-busy toggles correctly while loading.
* HTTP client tests (3, mocked fetch or MSW): apiPost builds http://localhost:5000/api/auth/login; validation-style ProblemDetails errors shows the first validation message; non-JSON/empty responses produce “Invalid server response”.
* E2E tests (2, Playwright): successful login (admin/password) shows token; invalid login shows the error message.

## Part 2 Minimal implementation

Backend

* Endpoint: POST /api/auth/login
* Valid credentials are configured in appsettings (admin/password in this repo)
* Success: 200 and { token: "hardcoded-token" }
* Failure: 401 and ProblemDetails

Frontend

* Route: /login
* Username and password fields
* Login button
* Shows one state

  * Loading
  * Success with token
  * Error with message

## How to run

Backend
cd backend
dotnet restore
dotnet run

Frontend
cd frontend
npm install
npm run dev

Open [http://localhost:3000/login](http://localhost:3000/login)

## Part 3 GCP notes

Deployment strategy

* C# API: Cloud Run
* Next.js app: Firebase Hosting
  * For this assignment, I kept the frontend as a client-side Next.js pages app so it can be deployed as a static site.
  * Because it’s client-only, Firebase Hosting is enough (deploy it as static files). If we later add SSR/server routes, we’d deploy the Next.js server on Cloud Run 

JWT validation on Cloud Run without calling the IdP every request

* Validate signatures locally using the IdP public keys from JWKS
* Use OIDC discovery to find the JWKS URL
* Cache keys in memory and refresh periodically or on rotation
* Validate issuer, audience, exp, nbf on each request

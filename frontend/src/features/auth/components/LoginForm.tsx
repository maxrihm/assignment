import { useReducer, useState } from "react"
import { login } from "../api"
import { ResponseError } from "../../../lib/http/errors"

type State =
  | { status: "idle" }
  | { status: "loading" }
  | { status: "success"; token: string }
  | { status: "error"; message: string }

type Action =
  | { type: "SUBMIT" }
  | { type: "SUCCESS"; token: string }
  | { type: "ERROR"; message: string }
  | { type: "RESET_ERROR" }

function reducer(state: State, action: Action): State {
  switch (action.type) {
    case "SUBMIT":
      return { status: "loading" }
    case "SUCCESS":
      return { status: "success", token: action.token }
    case "ERROR":
      return { status: "error", message: action.message }
    case "RESET_ERROR":
      return state.status === "error" ? { status: "idle" } : state
    default:
      return state
  }
}

export function LoginForm() {
  const [username, setUsername] = useState("")
  const [password, setPassword] = useState("")
  const [state, dispatch] = useReducer(reducer, { status: "idle" })

  const trimmedUsername = username.trim()
  const isLoading = state.status === "loading"
  const canSubmit = trimmedUsername.length > 0 && password.length > 0 && !isLoading

  const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    dispatch({ type: "SUBMIT" })

    try {
      const res = await login({ username: trimmedUsername, password })
      dispatch({ type: "SUCCESS", token: res.token })
    } catch (error) {
      const message =
        error instanceof ResponseError ? error.message : "Something went wrong"
      dispatch({ type: "ERROR", message })
    }
  }

  const onUsernameChange = (value: string) => {
    setUsername(value)
    dispatch({ type: "RESET_ERROR" })
  }

  const onPasswordChange = (value: string) => {
    setPassword(value)
    dispatch({ type: "RESET_ERROR" })
  }

  return (
    <div className="card" aria-busy={isLoading}>
      <h1>Sign In</h1>
      <p>Enter your username and password to continue.</p>

      <form onSubmit={onSubmit}>
        <div className="field">
          <label className="label" htmlFor="username">
            Username
          </label>
          <input
            className="input"
            id="username"
            name="username"
            type="text"
            value={username}
            onChange={(event) => onUsernameChange(event.target.value)}
            autoComplete="username"
            required
            disabled={isLoading}
          />
        </div>

        <div className="field">
          <label className="label" htmlFor="password">
            Password
          </label>
          <input
            className="input"
            id="password"
            name="password"
            type="password"
            value={password}
            onChange={(event) => onPasswordChange(event.target.value)}
            autoComplete="current-password"
            required
            disabled={isLoading}
          />
        </div>

        <button className="button" type="submit" disabled={!canSubmit}>
          {isLoading ? "Logging in..." : "Login"}
        </button>
      </form>

      {state.status === "loading" && (
        <p className="status" role="status" aria-live="polite">
          Loading...
        </p>
      )}
      {state.status === "success" && (
        <p className="status status-success" role="status" aria-live="polite">
          Success! Token: {state.token}
        </p>
      )}
      {state.status === "error" && (
        <p className="status status-error" role="alert" aria-live="assertive">
          Error: {state.message}
        </p>
      )}
    </div>
  )
}



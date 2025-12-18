import { ResponseError } from "../../lib/http/errors"
import type { ProblemDetails } from "../../lib/http/types"
import type { LoginRequest, LoginResponse } from "./types"

const API_BASE_URL = "http://localhost:5000"

export async function login(req: LoginRequest): Promise<LoginResponse> {
  const url = new URL("/api/auth/login", API_BASE_URL).toString()

  const response: Response = await fetch(url, {
    method: "POST",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    body: JSON.stringify(req),
  }).catch(() => {
    throw new ResponseError("Network error")
  })

  if (!response.ok) {
    const problem: ProblemDetails = await response.json()
    const message = problem.detail ?? problem.title
    throw new ResponseError(message, response.status)
  }

  const data: LoginResponse = await response.json()
  return data
}



import type { ProblemDetails } from "./types"
import { ResponseError } from "./errors"

const API_BASE_URL = "http://localhost:5000"

export async function apiPost<TResult>(
  path: string,
  body: unknown
): Promise<TResult> {
  const url = new URL(path, API_BASE_URL).toString()

  let response: Response
  try {
    response = await fetch(url, {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify(body),
    })
  } catch {
    throw new ResponseError("Network error")
  }

  const payload = await readJson(response)

  if (!response.ok) {
    const problem = (payload as ProblemDetails) ?? null
    throw new ResponseError(
      getErrorMessage(problem, response.status),
      response.status
    )
  }

  if (payload === null) {
    throw new ResponseError("Invalid server response", response.status)
  }

  return payload as TResult
}

async function readJson(response: Response): Promise<unknown> {
  if (response.status === 204) {
    return null
  }

  try {
    return await response.json()
  } catch {
    return null
  }
}

function getErrorMessage(problemDetails: ProblemDetails | null, status: number): string {
  if (problemDetails?.errors && typeof problemDetails.errors === "object") {
    const firstError = Object.values(problemDetails.errors)[0]?.[0]
    if (typeof firstError === "string" && firstError.length > 0) {
      return firstError
    }
  }

  if (typeof problemDetails?.detail === "string" && problemDetails.detail.length > 0) {
    return problemDetails.detail
  }

  if (typeof problemDetails?.title === "string" && problemDetails.title.length > 0) {
    return problemDetails.title
  }

  return `Request failed with status ${status}`
}



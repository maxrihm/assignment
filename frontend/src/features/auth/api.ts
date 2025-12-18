import { apiPost } from "../../lib/http/client"
import type { LoginRequest, LoginResponse } from "./types"

export async function login(req: LoginRequest): Promise<LoginResponse> {
  return apiPost<LoginResponse>("/api/auth/login", req)
}



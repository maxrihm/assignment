export type ProblemDetails = {
  title: string
  status: number
  type?: string
  detail?: string
  instance?: string
  traceId?: string
  errors?: Record<string, string[]>
}



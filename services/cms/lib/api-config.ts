export const PRODUCTION_API_BASE_URL = "https://api.eduardteodor.co.uk/v1";
export const DEVELOPMENT_API_BASE_URL = "http://localhost:8080/v1";

export function resolveApiBaseUrl(environment: string | undefined, override?: string): string {
  return override?.trim() || (environment === "production"
    ? PRODUCTION_API_BASE_URL
    : DEVELOPMENT_API_BASE_URL);
}

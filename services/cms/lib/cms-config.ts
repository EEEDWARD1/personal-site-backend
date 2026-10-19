export const PRODUCTION_CMS_ORIGIN = "https://cms.eduardteodor.co.uk";

export function resolveCmsOrigin(environment: string | undefined, override?: string): string {
  return override?.trim() || (environment === "production"
    ? PRODUCTION_CMS_ORIGIN
    : "http://localhost:3000");
}

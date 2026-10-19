import "server-only";
import { auth } from "@clerk/nextjs/server";
import { forwardPostRequest } from "./post-proxy-core";
import { resolveApiBaseUrl } from "./api-config";
import { resolveCmsOrigin } from "./cms-config";

export async function proxyPostRequest(request: Request, id?: string) {
  const session = await auth();
  return forwardPostRequest(request, id, {
    userId: session.userId,
    getToken: () => session.getToken(),
    baseUrl: resolveApiBaseUrl(process.env.NODE_ENV, process.env.API_BASE_URL),
    origin: resolveCmsOrigin(process.env.NODE_ENV, process.env.CMS_ORIGIN),
  });
}

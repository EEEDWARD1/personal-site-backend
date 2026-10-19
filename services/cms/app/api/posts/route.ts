import { proxyPostRequest } from "@/lib/post-proxy";

export const GET = (request: Request) => proxyPostRequest(request);
export const POST = (request: Request) => proxyPostRequest(request);

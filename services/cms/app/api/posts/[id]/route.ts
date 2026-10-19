import { proxyPostRequest } from "@/lib/post-proxy";

async function handle(request: Request, context: RouteContext<"/api/posts/[id]">) {
  const { id } = await context.params;
  return proxyPostRequest(request, id);
}

export { handle as GET, handle as PUT, handle as DELETE };

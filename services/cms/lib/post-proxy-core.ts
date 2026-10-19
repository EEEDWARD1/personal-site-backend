import { isAdminUser } from "./access.ts";

type ProxyOptions = {
  userId: string | null;
  getToken: () => Promise<string | null>;
  baseUrl: string;
  origin?: string;
  fetcher?: typeof fetch;
};

function problem(status: number, title: string) {
  return Response.json({ title }, { status, headers: { "Cache-Control": "no-store" } });
}

export async function forwardPostRequest(request: Request, id: string | undefined, options: ProxyOptions) {
  if (!options.userId) return problem(401, "Sign in required");
  if (!isAdminUser(options.userId)) return problem(403, "Access denied");

  if (id !== undefined && !/^[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}$/i.test(id))
    return problem(400, "Invalid post ID");

  const methods = id === undefined ? ["GET", "POST"] : ["GET", "PUT", "DELETE"];
  if (!methods.includes(request.method)) return problem(405, "Method not allowed");

  if (request.method !== "GET" && request.headers.get("origin") !== (options.origin ?? new URL(request.url).origin))
    return problem(403, "Cross-origin writes are not allowed");

  const hasBody = request.method === "POST" || request.method === "PUT";
  if (hasBody && request.headers.get("content-type")?.split(";")[0].trim() !== "application/json")
    return problem(415, "Send JSON content");

  const token = await options.getToken();
  if (!token) return problem(401, "Your session expired. Sign in again.");

  try {
    const base = new URL(options.baseUrl);
    if (base.username || base.password || base.search || base.hash ||
        (base.protocol !== "https:" && !(base.protocol === "http:" &&
          (["localhost", "127.0.0.1", "[::1]"].includes(base.hostname) ||
            base.origin === "http://api:8080"))))
      return problem(500, "API_BASE_URL must use HTTPS, loopback HTTP, or the private Docker API at http://api:8080");

    const url = new URL(`${options.baseUrl.replace(/\/+$/, "")}/admin/posts${id ? `/${id}` : ""}`);
    if (!id && request.method === "GET") {
      const query = new URL(request.url).searchParams;
      for (const key of ["page", "pageSize"]) {
        const value = query.get(key);
        if (value !== null) url.searchParams.set(key, value);
      }
    }

    const headers = new Headers({ Authorization: `Bearer ${token}` });
    if (hasBody) headers.set("Content-Type", "application/json");
    const ifMatch = request.headers.get("if-match");
    if (request.method === "PUT" && ifMatch) headers.set("If-Match", ifMatch);

    const upstream = await (options.fetcher ?? fetch)(url, {
      method: request.method,
      headers,
      body: hasBody ? await request.text() : undefined,
      cache: "no-store",
      redirect: "error",
      signal: AbortSignal.timeout(15_000),
    });
    const responseHeaders = new Headers({ "Cache-Control": "no-store" });
    for (const key of ["Content-Type", "ETag"]) {
      const value = upstream.headers.get(key);
      if (value) responseHeaders.set(key, value);
    }
    return new Response(upstream.status === 204 ? null : await upstream.text(), {
      status: upstream.status,
      headers: responseHeaders,
    });
  } catch {
    return problem(502, "Cannot reach the API. Check API_BASE_URL and that the .NET service is running.");
  }
}

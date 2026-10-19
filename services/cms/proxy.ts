import { clerkMiddleware } from "@clerk/nextjs/server";
import { NextResponse } from "next/server";
import { isAdminUser } from "./lib/access";
import { resolveCmsOrigin } from "./lib/cms-config";

// Early redirects only; pages and API handlers independently authorize access.
const isPrivate = (pathname: string) => ["/dashboard", "/api/posts"]
  .some(prefix => pathname === prefix || pathname.startsWith(`${prefix}/`));

export default clerkMiddleware(async (auth, request) => {
  if (request.nextUrl.pathname === "/sign-up" || request.nextUrl.pathname.startsWith("/sign-up/")) {
    return NextResponse.redirect(new URL("/sign-in", resolveCmsOrigin(process.env.NODE_ENV, process.env.CMS_ORIGIN)));
  }
  if (!isPrivate(request.nextUrl.pathname)) return;

  const { userId } = await auth();
  if (!userId) {
    if (request.nextUrl.pathname.startsWith("/api/"))
      return NextResponse.json({ title: "Sign in required" }, { status: 401, headers: { "Cache-Control": "no-store" } });
    await auth.protect();
  }
  if (!isAdminUser(userId)) {
    return NextResponse.json(
      { title: "Access denied. Only the CMS owner can access this resource." },
      { status: 403, headers: { "Cache-Control": "no-store" } },
    );
  }
}, { authorizedParties: [resolveCmsOrigin(process.env.NODE_ENV, process.env.CMS_ORIGIN)] });

export const config = {
  matcher: [
    "/((?!_next|[^?]*\\.(?:html?|css|js(?!on)|jpe?g|webp|png|gif|svg|ttf|woff2?|ico|csv|docx?|xlsx?|zip|webmanifest)).*)",
    "/(api|trpc)(.*)",
    "/__clerk/(.*)",
  ],
};

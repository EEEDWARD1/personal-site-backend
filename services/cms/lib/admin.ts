import "server-only";
import { auth } from "@clerk/nextjs/server";
import { notFound } from "next/navigation";
import { isAdminUser } from "./access";

export async function requireAdmin() {
  const session = await auth.protect();
  if (!isAdminUser(session.userId)) notFound();
  return session;
}

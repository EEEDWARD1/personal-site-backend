import type { ReactNode } from "react";
import { pageMetadata } from "@/app/_lib/metadata";

export const metadata = pageMetadata({
  title: "Admin",
  description:
    "Admin content management for Eduard Teodor's projects, notes, and freelance work.",
});

export default function AdminLayout({ children }: { children: ReactNode }) {
  return children;
}

import { requireAdmin } from "@/lib/admin";
import PostWorkspace from "./post-workspace";

export default async function Dashboard() {
  await requireAdmin();

  return (
    <main className="mx-auto w-full max-w-6xl px-6 py-8">
      <PostWorkspace />
    </main>
  );
}

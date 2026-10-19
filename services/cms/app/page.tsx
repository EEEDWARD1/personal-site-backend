import { Show, SignInButton } from "@clerk/nextjs";
import Link from "next/link";

export default function Home() {
  return (
    <main className="mx-auto w-full max-w-5xl space-y-4 px-6 py-12">
      <h1 className="text-2xl font-semibold">Posts</h1>
      <Show when="signed-out">
        <p>Private CMS. Sign in with your existing account.</p>
        <SignInButton>
          <button className="cursor-pointer underline">Sign in</button>
        </SignInButton>
      </Show>
      <Show when="signed-in">
        <Link href="/dashboard" className="underline">Open posts</Link>
      </Show>
    </main>
  );
}

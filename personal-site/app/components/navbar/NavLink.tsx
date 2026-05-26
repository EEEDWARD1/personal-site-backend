import Link from "next/link";
import { cn } from "@/app/lib/cn";

interface NavLinkProps {
  href: string;
  children: React.ReactNode;
  mobile?: boolean;
}

export default function NavLink({
  href,
  children,
  mobile = false,
}: NavLinkProps) {
  return (
    <li>
      <Link
        href={href}
        className={cn(
          "block rounded-md px-3 py-2 text-sm font-medium text-slate-700 transition-colors hover:bg-slate-100 hover:text-slate-950 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-accent",
          mobile && "w-full",
        )}
      >
        {children}
      </Link>
    </li>
  );
}

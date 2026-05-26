import Link from "next/link";

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
                className={mobile ? "block py-2 px-4 text-sm" : "py-2 px-4 text-sm"}
            >
                {children}
            </Link>
        </li>
    )
}
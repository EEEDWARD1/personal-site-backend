import NavLink from "./NavLink";
import {navigation} from "@/app/data/navigation";

export default function DesktopNavbar() {
    return (
        <ul className="hidden md:flex items-center gap-8">
            {navigation.map((item) => (
                <NavLink key={item.href} href={item.href}>
                    {item.label}
                </NavLink>
            ))}
        </ul>
    )
}
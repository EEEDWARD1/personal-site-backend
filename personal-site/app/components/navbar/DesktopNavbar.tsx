import NavLink from "./NavLink";
import { navigation } from "@/app/data/navigation";

export default function DesktopNavbar() {
  return (
    <ul className="hidden items-center gap-1 md:flex">
      {navigation.map((item) => (
        <NavLink key={item.href} href={item.href}>
          {item.label}
        </NavLink>
      ))}
    </ul>
  );
}

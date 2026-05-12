import { routes } from "@/shared/routes";
import Link from "next/link";
import { SidebarTrigger } from "../ui/sidebar";

export default function Header() {
  return (
    <header className="sticky top-0 z-50 w-full">
      <div className="container flex h-16 items-center px-4">
        <SidebarTrigger className="md:hidden pr-3" />
        <Link href={routes.home} className="flex items-center gap-3 group">
          <div className="relative flex h-9 w-9 items-center justify-center rounded-xl bg-linear-to-br from-primary via-primary to-chart-2 text-primary-foreground shadow-lg shadow-primary/25 group-hover:shadow-xl group-hover:shadow-primary/30 group-hover:scale-102 transition-all duration-200">
            <span className="text-lg font-black tracking-tight">DS</span>
            <div className="absolute inset-0 rounded-2xl bg-linear-to-br from-white/20 via-white/10 to-transparent" />
          </div>
          <span className="text-xl font-bold tracking-tight text-foreground group-hover:text-primary transition-colors">
            DirectoryService
          </span>
        </Link>
      </div>
    </header>
  );
}

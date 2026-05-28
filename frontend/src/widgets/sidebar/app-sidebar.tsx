"use client";

import Link from "next/link";
import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  useSidebar,
} from "../../shared/components/ui/sidebar";
import { routes } from "@/shared/routes";
import { Building2, Home, Map } from "lucide-react";
import { usePathname } from "next/navigation";

const menuItems = [
  { href: routes.home, label: "Home", icon: Home },
  { href: routes.departments, label: "Departments", icon: Building2 },
  { href: routes.locations, label: "Locations", icon: Map },
];

export function AppSidebar() {
  const pathname = usePathname();
  const { setOpenMobile, toggleSidebar, state, isMobile, openMobile } =
    useSidebar();

  const showLabel = isMobile ? openMobile : state === "expanded";

  return (
    <Sidebar variant="floating" collapsible="icon">
      <SidebarHeader className="mt-2">
        <button
          onClick={toggleSidebar}
          className="group flex w-full items-center gap-2 rounded-md p-2 text-left transition-colors hover:bg-sidebar-accent cursor-pointer"
        >
          <div className="relative flex h-8 w-8 shrink-0 items-center justify-center rounded-4xl bg-linear-to-br from-primary via-primary to-chart-2 text-primary-foreground shadow-lg shadow-primary/25 group-hover:shadow-xl group-hover:shadow-primary/30 group-hover:scale-102 transition-all duration-200">
            <span className="text-sm font-black tracking-tight">DS</span>
            <div className="absolute inset-0 rounded-4xl bg-linear-to-br from-white/20 via-white/10 to-transparent" />
          </div>
          {showLabel && (
            <span className="text-lg font-bold tracking-tight text-sidebar-foreground transition-colors overflow-hidden whitespace-nowrap">
              DirectoryService
            </span>
          )}
        </button>
      </SidebarHeader>
      <SidebarContent>
        <SidebarGroup className="mt-8">
          <SidebarGroupContent>
            <SidebarMenu className="flex gap-2">
              {menuItems.map((item) => {
                const isActive =
                  pathname === item.href ||
                  pathname.startsWith(item.href + "/");

                return (
                  <SidebarMenuItem key={item.href}>
                    <SidebarMenuButton
                      isActive={isActive}
                      tooltip={item.label}
                      onClick={() => setOpenMobile(false)}
                    >
                      <Link
                        href={item.href}
                        className="flex items-center gap-2 text-lg"
                      >
                        <item.icon className="" />
                        <span className="font-sans">{item.label}</span>
                      </Link>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                );
              })}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
    </Sidebar>
  );
}

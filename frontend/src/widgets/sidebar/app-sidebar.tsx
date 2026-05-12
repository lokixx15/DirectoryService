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
  SidebarTrigger,
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
  const { setOpenMobile } = useSidebar();

  return (
    <Sidebar variant="floating" collapsible="icon">
      <SidebarHeader>
        <SidebarTrigger className="mt-1 pl-1" />
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

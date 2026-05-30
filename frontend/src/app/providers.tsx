"use client";

import { queryClient } from "@/shared/api/query-client";
import { SidebarProvider } from "@/shared/components/ui/sidebar";
import { TooltipProvider } from "@/shared/components/ui/tooltip";
import { QueryClientProvider } from "@tanstack/react-query";
import { Toaster } from "@/shared/components/ui/sonner";

export default function AppLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <QueryClientProvider client={queryClient}>
      <SidebarProvider defaultOpen={false}>
        <TooltipProvider>
          {children}
          <Toaster />
        </TooltipProvider>
      </SidebarProvider>
    </QueryClientProvider>
  );
}

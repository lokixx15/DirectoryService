import {
  CalendarIcon,
  HashIcon,
  FolderTreeIcon,
  CircleCheckIcon,
  CircleXIcon,
} from "lucide-react";

import { Card, CardContent, CardHeader } from "@/shared/components/ui/card";
import { Badge } from "@/shared/components/ui/badge";
import { Department } from "@/entities/departments/types";
import { FormatDate } from "@/shared/lib/format-date";
import { ReactNode } from "react";
import { cn } from "@/shared/lib/utils";

interface DepartmentTreeCardProps {
  department: Department;
  children?: ReactNode;
  isSelected?: boolean;
  onClick?: () => void;
}

export function DepartmentTreeCard({
  department,
  children,
  isSelected,
  onClick,
}: DepartmentTreeCardProps) {
  return (
    <Card
      onClick={onClick}
      className={cn(
        "border-l-4 transition-colors mb-0.5 cursor-pointer select-none",
        isSelected
          ? "border-l-primary bg-primary/5"
          : "border-l-primary/30 hover:border-l-primary hover:bg-muted/40",
      )}
    >
      <CardHeader className="flex flex-row items-center justify-between pb-2 mt-[-10]">
        <div className="flex items-center gap-2 min-w-0">
          <FolderTreeIcon className="size-4 shrink-0 text-muted-foreground" />
          <span className="font-semibold truncate">{department.name}</span>
          <Badge variant="secondary" className="shrink-0 text-xs">
            {department.identifier}
          </Badge>
          {!department.isActive && (
            <span className="text-[10px] text-muted-foreground">inactive</span>
          )}
        </div>

        {children && <div onClick={(e) => e.stopPropagation()}>{children}</div>}
      </CardHeader>

      <CardContent className="pb-3">
        <div className="flex flex-wrap items-center gap-x-4 gap-y-1 text-xs text-muted-foreground">
          <span className="flex items-center gap-1">
            <HashIcon className="size-3" />
            depth: {department.depth}
          </span>
          <span className="flex items-center gap-1">
            <FolderTreeIcon className="size-3" />
            {department.path}
          </span>
          <span className="flex items-center gap-1">
            <CalendarIcon className="size-3" />
            {FormatDate(department.createdAt)}
          </span>
          <span className="flex items-center gap-1">
            <CalendarIcon className="size-3" />
            {FormatDate(department.updatedAt)}
          </span>
          {department.isActive ? (
            <CircleCheckIcon className="size-3 text-green-600" />
          ) : (
            <CircleXIcon className="size-3 text-red-600" />
          )}
        </div>
      </CardContent>
    </Card>
  );
}

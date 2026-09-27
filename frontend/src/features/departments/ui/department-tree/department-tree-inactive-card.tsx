import { CalendarIcon, FolderTreeIcon } from "lucide-react";
import { Card, CardContent, CardHeader } from "@/shared/components/ui/card";
import { Department } from "@/entities/departments/types";
import { FormatDate } from "@/shared/lib/format-date";
import { ActivitySwitcher } from "@/shared/components/switches/activity-switcher";
import { ReactNode } from "react";

interface DepartmentTreeInactiveCardProps {
  department: Department;
  onClick?: () => void;
  onIsActiveChange: (isActive: boolean) => void;
  children?: ReactNode;
}

export function DepartmentTreeInactiveCard({
  department,
  onClick,
  onIsActiveChange,
  children,
}: DepartmentTreeInactiveCardProps) {
  return (
    <Card
      onClick={onClick}
      className="border-l-4 transition-colors mb-0.5 cursor-pointer select-none"
    >
      <CardHeader className="flex flex-row items-center justify-between pb-2 mt-[-10]">
        <div className="flex items-center gap-2 min-w-0">
          <FolderTreeIcon className="size-4 shrink-0 text-muted-foreground" />
          <span className="font-semibold truncate">{department.name}</span>
        </div>

        {children && <div onClick={(e) => e.stopPropagation()}>{children}</div>}
      </CardHeader>

      <CardContent className="pb-3">
        <div className="flex flex-wrap items-center gap-x-4 gap-y-1 text-xs text-muted-foreground">
          <span className="flex items-center gap-1">
            <CalendarIcon className="size-3" />
            {FormatDate(department.deletedAt)}
          </span>
          <ActivitySwitcher
            isActive={department.isActive}
            onIsActiveChange={onIsActiveChange}
          />
        </div>
      </CardContent>
    </Card>
  );
}

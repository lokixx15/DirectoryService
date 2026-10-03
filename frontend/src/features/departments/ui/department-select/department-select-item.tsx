"use client";

import { DepartmentStandard } from "@/entities/departments/types";
import { Badge } from "@/shared/components/ui/badge";
import { Button } from "@/shared/components/ui/button";
import { FormatDate } from "@/shared/lib/format-date";
import { Info } from "lucide-react";
import { useState } from "react";

interface DepartmentSelectItemProps {
  department: DepartmentStandard;
}

export function DepartmentSelectItem({
  department,
}: DepartmentSelectItemProps) {
  const [isInfoVisible, setIsInfoVisible] = useState(false);

  return (
    <div className="flex justify-between w-full items-center gap-2">
      <div className="flex flex-col min-w-0">
        <span className="font-medium text-sm truncate">{department.name}</span>
        <div className="flex items-center gap-1 text-xs text-muted-foreground">
          <span className="truncate">{department.path}</span>
          <Badge
            variant={department.isActive ? "success" : "destructive"}
            className="w-2 h-2 rounded-full p-0 shrink-0"
          />
        </div>
      </div>

      {isInfoVisible && (
        <div className="grid grid-cols-1 gap-1 text-[11px] text-muted-foreground whitespace-nowrap">
          <span>Created: {FormatDate(department.createdAt)}</span>
          <span>Updated: {FormatDate(department.updatedAt)}</span>
          {department.deletedAt && (
            <span>Deleted: {FormatDate(department.deletedAt)}</span>
          )}
        </div>
      )}

      <div className="flex gap-0.5 shrink-0">
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="rounded-full hover:bg-gray-200 text-gray-600 hover:text-gray-700"
          onClick={(e) => {
            e.preventDefault();
            setIsInfoVisible((prev) => !prev);
          }}
        >
          <Info className="h-7 w-7" />
        </Button>
      </div>
    </div>
  );
}

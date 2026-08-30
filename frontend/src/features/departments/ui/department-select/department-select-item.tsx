"use client";

import { DepartmentStandard } from "@/entities/departments/types";
import { Badge } from "@/shared/components/ui/badge";
import { Button } from "@/shared/components/ui/button";
import { FormatDate } from "@/shared/lib/format-date";
import { CirclePlus, CircleMinus, Info } from "lucide-react";
import { useState } from "react";

interface DepartmentSelectItemProps {
  department: DepartmentStandard;
  onAddClick: (departmentId: string) => void;
  onExcludeClick: (departmentId: string) => void;
}

export function DepartmentSelectItem({
  department,
  onAddClick,
  onExcludeClick,
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
        <div className="flex gap-2 text-[11px] text-muted-foreground whitespace-nowrap">
          <span>C: {FormatDate(department.createdAt)}</span>
          <span>U: {FormatDate(department.updatedAt)}</span>
          {department.deletedAt && <span>D: {FormatDate(department.deletedAt)}</span>}
        </div>
      )}

      <div className="flex gap-0.5 shrink-0">
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="rounded-full hover:bg-green-100 text-green-600 hover:text-green-700"
          onClick={() => onAddClick(department.id)}
        >
          <CirclePlus className="h-7 w-7" />
        </Button>
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="rounded-full hover:bg-red-100 text-red-600 hover:text-red-700"
          onClick={() => onExcludeClick(department.id)}
        >
          <CircleMinus className="h-7 w-7" />
        </Button>
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="rounded-full hover:bg-gray-200 text-gray-600 hover:text-gray-700"
          onClick={() => setIsInfoVisible((prev) => !prev)}
        >
          <Info className="h-7 w-7" />
        </Button>
      </div>
    </div>
  );
}

"use client";

import { Button } from "@/shared/components/ui/button";
import { DepartmentStandard } from "@/entities/departments/types";
import { X } from "lucide-react";

interface DepartmentBadgeSectionProps {
  departments: DepartmentStandard[];
  onRemove: (id: string) => void;
  onClear: () => void;
  variant: "added" | "excluded";
}

const variantStyles = {
  added: "bg-primary/10 text-primary",
  excluded: "bg-destructive/10 text-destructive",
} as const;

export function DepartmentBadgeSection({
  departments,
  onRemove,
  onClear,
  variant,
}: DepartmentBadgeSectionProps) {
  return (
    <div className="flex flex-wrap items-center gap-1.5 min-w-0">
      {departments.map((dep) => (
        <span
          key={dep.id}
          className={`inline-flex items-center gap-1 rounded-md px-2 py-0.5 text-xs font-medium whitespace-nowrap ${variantStyles[variant]}`}
        >
          {dep.name}
          <Button
            onClick={() => onRemove(dep.id)}
            variant="secondary"
            className="rounded-sm opacity-60 hover:opacity-100 h-auto w-2 p-0"
          >
            <X className="h-3 w-3" />
          </Button>
        </span>
      ))}
      {departments.length > 0 && (
        <Button
          onClick={onClear}
          variant="destructive"
          size="xs"
          className="text-xs"
        >
          Clear all
        </Button>
      )}
    </div>
  );
}

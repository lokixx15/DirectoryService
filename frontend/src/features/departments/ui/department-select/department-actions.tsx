"use client";

import { Button } from "@/shared/components/ui/button";
import { LoadMoreButton } from "@/shared/components/pagination/load-more-button";

interface DepartmentActionsProps {
  totalCount: number;
  pageSize: number;
  isFetching: boolean;
  showApply: boolean;
  onPageSizeChange: (size: number) => void;
  onApply: () => void;
}

export function DepartmentActions({
  totalCount,
  pageSize,
  isFetching,
  showApply,
  onPageSizeChange,
  onApply,
}: DepartmentActionsProps) {
  return (
    <div className="flex flex-col items-center gap-1">
      {totalCount > pageSize && (
        <LoadMoreButton
          totalElements={totalCount}
          pageSize={pageSize}
          onPageSizeChange={onPageSizeChange}
          className="w-full"
          loading={isFetching}
        />
      )}
      {showApply && (
        <Button onClick={onApply} variant="creative" className="mt-1">
          Apply
        </Button>
      )}
    </div>
  );
}

"use client";

import { useState } from "react";
import { Button } from "@/shared/components/ui/button";
import { usePagination } from "@/shared/hooks/use-pagination";
import { ChevronDown } from "lucide-react";
import { ErrorCard } from "@/shared/components/errors/error-card";
import { SkeletonCard } from "@/shared/components/skeletons/skeleton-card";
import { NotFoundCard } from "@/shared/components/cards/not-found-card";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/shared/components/ui/dropdown-menu";
import { useDebounce } from "@/shared/hooks/use-debounce";
import { useDepartmentSummaryList } from "../../model/use-department-summary-list";
import { DepartmentSummary } from "@/entities/departments";
import { LoadMoreButton } from "@/shared/components/pagination/load-more-button";
import { SearchBar } from "@/shared/components/search/search-bar";

interface DepartmentSingleSelectProps {
  excludedId?: string;
  department?: DepartmentSummary;
  isRootSelected: boolean;
  onDepartmentChange: (department?: DepartmentSummary) => void;
}

export function DepartmentSingleSelect({
  excludedId,
  department,
  isRootSelected = false,
  onDepartmentChange,
}: DepartmentSingleSelectProps) {
  const { page, pageSize, onPageSizeChange } = usePagination(10);
  const [search, setSearch] = useState<string>();
  const debouncedSearch = useDebounce(search, 400);

  const [open, setOpen] = useState(false);

  const {
    departmentsSummary,
    isPending,
    errors,
    isError,
    totalCount,
    isFetching,
    refetch,
  } = useDepartmentSummaryList({
    search: debouncedSearch,
    page,
    pageSize,
    excludedId,
  });

  if (isError) {
    return <ErrorCard errors={errors ?? []} refetch={refetch} />;
  }

  if (isPending && !departmentsSummary) {
    return <SkeletonCard quantity={6} />;
  }

  return (
    <DropdownMenu open={open} onOpenChange={setOpen}>
      <DropdownMenuTrigger asChild>
        <Button
          variant="outline"
          className="group flex items-center justify-between max-w-100 gap-2 h-9"
        >
          <span>
            {isRootSelected
              ? "- (make as root)"
              : (department?.name ?? "Choose department")}
          </span>
          <ChevronDown className="h-4 w-4 opacity-50 transition-transform duration-200 rotate-180 group-data-[state=open]:rotate-0" />
        </Button>
      </DropdownMenuTrigger>

      <DropdownMenuContent className="p-4 mt-2 border border-solid border-primary-600 bg-popover rounded-md shadow-md w-(--radix-dropdown-menu-trigger-width)">
        <SearchBar onSearch={setSearch} className="mb-1.5" />
        <div className="flex flex-col gap-4">
          {departmentsSummary?.length ? (
            <DropdownMenuGroup className="flex flex-col w-full max-h-60 overflow-y-auto">
              <DropdownMenuItem
                className="flex items-start gap-1 p-2 justify-between rounded-sm m-1 cursor-default select-none outline-none transition-colors data-highlighted:bg-green-200"
                onSelect={() => onDepartmentChange(undefined)}
              >
                - (make as root)
              </DropdownMenuItem>
              {departmentsSummary.map((department) => {
                return (
                  <DropdownMenuItem
                    key={department.id}
                    className="flex items-start gap-1 p-2 justify-between rounded-sm m-1 cursor-default select-none outline-none transition-colors data-highlighted:bg-green-200"
                    onSelect={() => onDepartmentChange(department)}
                  >
                    <span>{department.name}</span>
                    <span className="ml-1 text-xs text-muted-foreground">
                      / {department.identifier}
                    </span>
                  </DropdownMenuItem>
                );
              })}
              <LoadMoreButton
                totalElements={totalCount}
                pageSize={pageSize}
                onPageSizeChange={onPageSizeChange}
                loading={isFetching}
              />
            </DropdownMenuGroup>
          ) : (
            <NotFoundCard
              title="Not found departments"
              description="Try adjusting your search or filters"
            />
          )}
        </div>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}

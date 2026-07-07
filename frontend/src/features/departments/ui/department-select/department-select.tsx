"use client";

import { ReactNode, useState } from "react";

import { Button } from "@/shared/components/ui/button";
import { usePagination } from "@/shared/hooks/use-pagination";
import { ChevronDown } from "lucide-react";
import { useDepartmentSelect } from "../../model/use-department-select";
import { ErrorCard } from "@/shared/components/errors/error-card";
import { useDepartments } from "../../model/use-departments";
import { SkeletonCard } from "@/shared/components/skeletons/skeleton-card";
import { NotFoundCard } from "@/shared/components/cards/not-found-card";
import { OrderState } from "@/shared/types/ordering";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from "@radix-ui/react-dropdown-menu";
import {
  DropdownMenuGroup,
  DropdownMenuItem,
} from "@/shared/components/ui/dropdown-menu";
import { DepartmentSelectItem } from "./department-select-item";
import { DepartmentFilters } from "./department-filters";
import { DepartmentBadgeSection } from "./department-badge-section";
import { DepartmentActions } from "./department-actions";
import { useDebounce } from "@/shared/hooks/use-debounce";

interface DepartmentSelectProps {
  locationIds: string[];
  filterActions?: ReactNode;
}

export function DepartmentSelect({
  locationIds,
  filterActions,
}: DepartmentSelectProps) {
  const { page, pageSize, onPageSizeChange } = usePagination(10);
  const [departmentIds, setDepartmentIds] = useState<string[]>([]);
  const [excludeDepartmentIds, setExcludeDepartmentIds] = useState<string[]>(
    [],
  );
  const [search, setSearch] = useState<string>();
  const debouncedSearch = useDebounce(search, 400);

  const [order, setOrder] = useState<OrderState>();
  const [isActive, setIsActive] = useState<boolean>();

  const {
    selectedAddedDepartments,
    selectedExcludedDepartments,
    addDepartment,
    removeAddedDepartment,
    clearSelectedAddedDepartments,
    addExcludedDepartment,
    removeExcludedDepartment,
    clearExcludedDepartments,
    applySelectedAddedDepartments,
    applySelectedExcludedDepartments,
  } = useDepartmentSelect({
    onAddedChange: setDepartmentIds,
    onExcludeChange: setExcludeDepartmentIds,
  });

  const {
    departments,
    isError,
    totalCount,
    errors,
    isFetching,
    isPending,
    refetch,
  } = useDepartments({
    search: debouncedSearch,
    departmentIds,
    locationIds,
    excludeDepartmentIds,
    isActive,
    page,
    pageSize,
    order,
  });

  if (isError) {
    return <ErrorCard errors={errors ?? []} refetch={refetch} />;
  }

  if (isPending && !departments) {
    return <SkeletonCard quantity={6} />;
  }

  const hasPendingSelections =
    selectedAddedDepartments.length > 0 ||
    selectedExcludedDepartments.length > 0 ||
    departmentIds.length > 0 ||
    excludeDepartmentIds.length > 0;

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button className="group flex items-center justify-between gap-2 w-full">
          <span>Choose departments</span>
          <ChevronDown className="h-4 w-4 opacity-50 transition-transform duration-200 rotate-180 group-data-[state=open]:rotate-0" />
        </Button>
      </DropdownMenuTrigger>

      <DropdownMenuContent className="p-4 mt-2 border border-solid border-primary-600 bg-popover rounded-md shadow-md w-(--radix-dropdown-menu-trigger-width)">
        <div className="flex flex-col gap-4">
          <DepartmentFilters
            onSearch={setSearch}
            onOrder={setOrder}
            onStatus={setIsActive}
            actions={filterActions}
          />

          {departments?.length ? (
            <DropdownMenuGroup className="w-full max-h-60 overflow-y-auto">
              {departments.map((department) => {
                const isAdded = selectedAddedDepartments.some(
                  (d) => d.id === department.id,
                );
                const isExcluded = selectedExcludedDepartments.some(
                  (d) => d.id === department.id,
                );

                return (
                  <DropdownMenuItem
                    onSelect={(e) => e.preventDefault()}
                    key={department.id}
                    className="flex items-start gap-1 p-2 justify-between rounded-sm m-1 cursor-default select-none outline-none transition-colors data-highlighted:bg-slate-50"
                    style={{
                      backgroundColor: isAdded
                        ? "#f0fdf4"
                        : isExcluded
                          ? "#fef2f2"
                          : undefined,
                    }}
                  >
                    <DepartmentSelectItem
                      department={department}
                      onAddClick={addDepartment}
                      onExcludeClick={addExcludedDepartment}
                    />
                  </DropdownMenuItem>
                );
              })}
            </DropdownMenuGroup>
          ) : (
            <NotFoundCard
              title="Not found departments"
              description="Try adjusting your search or filters"
            />
          )}

          <div className="grid grid-cols-[1fr_auto_1fr] items-start w-full gap-2 pt-2 border-t border-border">
            <DepartmentBadgeSection
              departments={selectedAddedDepartments}
              onRemove={removeAddedDepartment}
              onClear={clearSelectedAddedDepartments}
              variant="added"
            />

            <DepartmentActions
              totalCount={totalCount}
              pageSize={pageSize}
              isFetching={isFetching}
              showApply={hasPendingSelections}
              onPageSizeChange={onPageSizeChange}
              onApply={() => {
                applySelectedAddedDepartments();
                applySelectedExcludedDepartments();
              }}
            />

            <DepartmentBadgeSection
              departments={selectedExcludedDepartments}
              onRemove={removeExcludedDepartment}
              onClear={clearExcludedDepartments}
              variant="excluded"
            />
          </div>
        </div>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}

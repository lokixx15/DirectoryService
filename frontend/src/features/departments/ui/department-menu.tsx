"use client";

import { useState } from "react";

import { Button } from "@/shared/components/ui/button";
import {
  Command,
  CommandDialog,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/shared/components/ui/command";
import { useDepartmentSummaryList } from "@/features/departments/model/use-department-summary-list";
import { usePagination } from "@/shared/hooks/use-pagination";
import { useDebounce } from "@/shared/hooks/use-debounce";
import { Building2, Check, X } from "lucide-react";
import { DialogTitle } from "@/shared/components/ui/dialog";
import { useDepartmentSelector } from "../model/use-department-selector";
import { LoadMoreButton } from "@/shared/components/pagination/load-more-button";

export function DepartmentMenu({
  onDepartmentIdsChange,
}: {
  onDepartmentIdsChange: (departmentIds: string[]) => void;
}) {
  const { page, pageSize, onPageSizeChange } = usePagination(10);
  const [search, setSearch] = useState("");
  const debouncedValue = useDebounce(search, 300);

  const { departmentsSummary, totalCount, isLoading, isError } =
    useDepartmentSummaryList({
      page,
      pageSize,
      search: debouncedValue,
    });

  const {
    open,
    selectedDepartments,
    appliedDepartments,
    removeDepartment,
    clearSelectedDepartments,
    applySelectedDepartments,
    onOpenDialogChange,
    toggleDepartment,
  } = useDepartmentSelector(onDepartmentIdsChange);
  const [selectedItemValue, setSelectedItemValue] = useState("");

  return (
    <div className="flex flex-col gap-4">
      <Button
        onClick={() => onOpenDialogChange(true)}
        variant="outline"
        className="w-fit gap-2"
      >
        <Building2 className="h-4 w-4" />
        Related departments
        {!open && appliedDepartments.length > 0 && (
          <span className="flex h-5 min-w-5 items-center justify-center rounded-full bg-primary px-1.5 text-[10px] font-medium text-primary-foreground">
            {appliedDepartments.length}
          </span>
        )}
      </Button>

      <CommandDialog open={open} onOpenChange={onOpenDialogChange}>
        <DialogTitle />
        <Command value={selectedItemValue} onValueChange={setSelectedItemValue}>
          <div className="flex flex-wrap items-center gap-1.5 border-b px-3 py-2 pr-10">
            {selectedDepartments.map((dep) => (
              <span
                key={dep.id}
                className="flex items-center gap-1 rounded-md bg-secondary px-2 py-0.5 text-xs font-medium text-secondary-foreground"
              >
                {dep.name}
                <Button
                  onClick={() => removeDepartment(dep.id)}
                  variant="secondary"
                  className="rounded-sm opacity-60 transition-opacity hover:opacity-100 h-auto w-2"
                >
                  <X className="h-3 w-3" />
                </Button>
              </span>
            ))}
            {selectedDepartments?.length > 0 && (
              <Button
                onClick={clearSelectedDepartments}
                variant="destructive"
                size="xs"
                className="text-xs"
              >
                Clear all
              </Button>
            )}
            <Button
              onClick={applySelectedDepartments}
              variant="creative"
              size="xs"
              className="text-xs"
            >
              Apply
            </Button>
          </div>

          <CommandInput
            placeholder="Type a department name..."
            onValueChange={setSearch}
          />
          <CommandList>
            <CommandGroup forceMount>
              {departmentsSummary?.map((dep) => {
                const selected = selectedDepartments.some(
                  (d) => d.id === dep.id,
                );

                return (
                  <CommandItem
                    key={dep.id}
                    onSelect={() => toggleDepartment(dep)}
                    className="data-[selected=true]:bg-background"
                  >
                    <div
                      className={`mr-2 grid h-4 w-4 shrink-0 place-content-center rounded-sm border ${
                        selected
                          ? "border-primary bg-primary text-primary-foreground"
                          : "border-input"
                      }`}
                    >
                      {selected && <Check className="h-3 w-3" />}
                    </div>
                    <span>{dep.name}</span>
                    <span className="ml-1 text-xs text-muted-foreground">
                      / {dep.identifier}
                    </span>
                  </CommandItem>
                );
              })}
              {!departmentsSummary?.length && (
                <CommandEmpty>No results found</CommandEmpty>
              )}
            </CommandGroup>
            <div className="p-2">
              {totalCount !== undefined && totalCount > 0 && (
                <LoadMoreButton
                  totalElements={totalCount}
                  pageSize={pageSize}
                  onPageSizeChange={onPageSizeChange}
                  onPointerEnter={() => setSelectedItemValue("")}
                  className="w-full"
                />
              )}
            </div>
          </CommandList>
        </Command>
      </CommandDialog>
    </div>
  );
}

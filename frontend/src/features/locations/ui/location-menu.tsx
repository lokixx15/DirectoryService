"use client";

import { ReactNode, useState } from "react";

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
import { usePagination } from "@/shared/hooks/use-pagination";
import { useDebounce } from "@/shared/hooks/use-debounce";
import { Check, X } from "lucide-react";
import { DialogTitle } from "@/shared/components/ui/dialog";
import { LoadMoreButton } from "@/shared/components/pagination/load-more-button";
import { ErrorLabel } from "@/shared/components/errors/error-label";
import { useLocationSummaryList } from "../model/use-location-summary-list";
import { useLocationMenu } from "../model/use-location-menu";

interface LocationMenuProps {
  onLocationIdsChange: (locationIds: string[]) => void;
  children: ReactNode;
}

export function LocationMenu({
  onLocationIdsChange,
  children,
}: LocationMenuProps) {
  const { page, pageSize, onPageSizeChange } = usePagination(10);
  const [search, setSearch] = useState("");
  const debouncedValue = useDebounce(search, 300);

  const { locationsSummary, totalCount, isFetching, isError, refetch } =
    useLocationSummaryList({
      page,
      pageSize,
      search: debouncedValue,
    });

  const {
    open,
    selectedLocations,
    appliedLocations,
    removeLocation,
    clearSelectedLocations,
    applySelectedLocations,
    onOpenDialogChange,
    toggleLocation,
  } = useLocationMenu(onLocationIdsChange);
  const [selectedItemValue, setSelectedItemValue] = useState("");

  return (
    <div className="flex flex-col gap-4">
      <Button
        onClick={() => onOpenDialogChange(true)}
        variant="outline"
        className="w-fit gap-2"
      >
        {children}
        {!open && appliedLocations.length > 0 && (
          <span className="flex h-5 min-w-5 items-center justify-center rounded-full bg-primary px-1.5 text-[10px] font-medium text-primary-foreground">
            {appliedLocations.length}
          </span>
        )}
      </Button>

      <CommandDialog open={open} onOpenChange={onOpenDialogChange}>
        <DialogTitle />
        <Command value={selectedItemValue} onValueChange={setSelectedItemValue}>
          <div className="flex flex-wrap items-center gap-1.5 border-b px-3 py-2 pr-10">
            {selectedLocations.map((dep) => (
              <span
                key={dep.id}
                className="flex items-center gap-1 rounded-md bg-secondary px-2 py-0.5 text-xs font-medium text-secondary-foreground"
              >
                {dep.name}
                <Button
                  onClick={() => removeLocation(dep.id)}
                  variant="secondary"
                  className="rounded-sm opacity-60 transition-opacity hover:opacity-100 h-auto w-2"
                >
                  <X className="h-3 w-3" />
                </Button>
              </span>
            ))}
            {selectedLocations?.length > 0 && (
              <Button
                onClick={clearSelectedLocations}
                variant="destructive"
                size="xs"
                className="text-xs"
              >
                Clear all
              </Button>
            )}
            <Button
              onClick={applySelectedLocations}
              variant="creative"
              size="xs"
              className="text-xs"
            >
              Apply
            </Button>
          </div>

          <CommandInput
            placeholder="Search locations..."
            onValueChange={setSearch}
          />
          {isError ? (
            <ErrorLabel refetch={refetch}>
              Error occured while loading
            </ErrorLabel>
          ) : (
            <CommandList>
              <CommandGroup forceMount>
                {locationsSummary?.map((dep) => {
                  const selected = selectedLocations.some(
                    (d) => d.id === dep.id,
                  );

                  return (
                    <CommandItem
                      key={dep.id}
                      onSelect={() => toggleLocation(dep)}
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
                        / {dep.timezone}
                      </span>
                    </CommandItem>
                  );
                })}
                {!isFetching && !locationsSummary?.length && (
                  <CommandEmpty>No results found</CommandEmpty>
                )}
              </CommandGroup>
              {((totalCount && totalCount > 0) || isFetching) && (
                <div className="p-2">
                  <LoadMoreButton
                    totalElements={totalCount ?? 0}
                    pageSize={pageSize}
                    onPageSizeChange={onPageSizeChange}
                    onPointerEnter={() => setSelectedItemValue("")}
                    className="w-full"
                    loading={isFetching}
                  />
                </div>
              )}
            </CommandList>
          )}
        </Command>
      </CommandDialog>
    </div>
  );
}

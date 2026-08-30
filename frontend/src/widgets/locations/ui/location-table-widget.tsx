"use client";

import {
  flexRender,
  getCoreRowModel,
  SortingState,
  useReactTable,
  VisibilityState,
} from "@tanstack/react-table";

import { PaginationIconsOnly } from "@/shared/components/pagination/pagination-icons-only";
import { SearchBar } from "@/shared/components/search/search-bar";
import { useMemo, useState } from "react";

import { ColumnsDropdown } from "@/shared/components/dropdowns/columns-dropdown";
import {
  CreateLocationDialog,
  EditLocationDialog,
  DeleteLocationDialog,
  useLocationList,
  createLocationColumns,
} from "@/features/locations";
import { Location } from "@/entities/locations";
import { usePagination } from "@/shared/hooks/use-pagination";
import { SkeletonTable } from "@/shared/components/skeletons/skeleton-table";
import { ErrorCard } from "@/shared/components/errors/error-card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/components/ui/table";
import { StatusFilter } from "@/shared/components/filters/status-filter";
import { DepartmentSelect } from "@/features/departments/ui/department-select/department-select";
import { Button } from "@/shared/components/ui/button";
import { useLocationFilters } from "@/features/locations/model/use-location-filters";
import { Skeleton } from "@/shared/components/ui/skeleton";

export function LocationTableWidget() {
  const { pageSize, onPageSizeChange } =
    usePagination(10);
  const [search, setSearch] = useState("");
  const [sorting, setSorting] = useState<SortingState>([]);
  const [isActive, setIsActive] = useState<boolean>();

  const {
    pageIndex,
    setPage,
    addedDepartmentIds,
    excludedDepartmentIds,
    setAddedDepartmentIdsHandler,
    setExcludedDepartmentIdsHandler,
  } = useLocationFilters();

  const [editingLocation, setEditingLocation] = useState<Location>();
  const [editOpen, setEditOpen] = useState<boolean>(false);

  const [deletingLocation, setDeletingLocation] = useState<Location>();
  const [deleteOpen, setDeleteOpen] = useState<boolean>(false);

  const { locations, totalCount, totalPages, isPending, errors, refetch } =
    useLocationList({
      page: pageIndex,
      pageSize,
      search,
      sorting,
      isActive,
      selectedDepartmentIds: addedDepartmentIds,
      excludedDepartmentIds,
    });

  const [columnVisibility, setColumnVisibility] = useState<VisibilityState>({
    id: false,
  });

  const columns = useMemo(
    () =>
      createLocationColumns(
        (location) => {
          setEditingLocation(location);
          setEditOpen(true);
        },
        (location) => {
          setDeletingLocation(location);
          setDeleteOpen(true);
        },
      ),
    [],
  );

  // eslint-disable-next-line react-hooks/incompatible-library
  const table = useReactTable({
    data: locations || [],
    columns,
    getCoreRowModel: getCoreRowModel(),
    manualPagination: true,
    manualSorting: true,
    enableSortingRemoval: false,
    onColumnVisibilityChange: setColumnVisibility,
    onSortingChange: setSorting,
    state: {
      pagination: {
        pageIndex,
        pageSize: pageSize,
      },
      columnVisibility,
      sorting,
    },
  });

  if (isPending) {
    return (
      <div>
        <Skeleton className="h-10 w-full bg-primary/10 mb-5" />
        <SkeletonTable />
      </div>
    );
  }

  if (errors?.length) {
    return <ErrorCard errors={errors} refetch={refetch} />;
  }

  return (
    <div className="flex flex-col gap-3">
      <div className="flex items-center w-full gap-2 flex-wrap">
        <SearchBar onSearch={setSearch} />
        <ColumnsDropdown table={table} />
        <StatusFilter onIsActive={setIsActive} />
        <DepartmentSelect
          key={addedDepartmentIds.join(",") + excludedDepartmentIds.join(",")}
          addedDepartmentIds={addedDepartmentIds}
          onAddedDepartmentIdsChange={setAddedDepartmentIdsHandler}
          excludedDepartmentIds={excludedDepartmentIds}
          onExcludedDepartmentIdsChange={setExcludedDepartmentIdsHandler}
        />
        <CreateLocationDialog />
      </div>

      <div className="overflow-hidden rounded-md border">
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id} className="border-b-foreground/50">
                {headerGroup.headers.map((header) => {
                  return (
                    <TableHead key={header.id}>
                      {header.isPlaceholder
                        ? null
                        : flexRender(
                            header.column.columnDef.header,
                            header.getContext(),
                          )}
                    </TableHead>
                  );
                })}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow
                  key={row.id}
                  data-state={row.getIsSelected() && "selected"}
                >
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext(),
                      )}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={table.getAllColumns().length}
                  className="h-24 text-center"
                >
                  <div className="flex flex-col items-center justify-center gap-3 w-full h-full">
                    <span className="text-muted-foreground font-medium text-sm">
                      No results.
                    </span>
                    <Button
                      variant="secondary"
                      size="sm"
                      onClick={() => {
                        setSearch("");
                        setIsActive(undefined);
                        setAddedDepartmentIdsHandler([]);
                        setExcludedDepartmentIdsHandler([]);
                        setPage(0);
                      }}
                    >
                      Clear Filters
                    </Button>
                  </div>
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>

      <PaginationIconsOnly
        page={pageIndex}
        pageSize={pageSize}
        totalCount={totalCount}
        totalPages={totalPages}
        onPageIndexChange={setPage}
        onPageSizeChange={onPageSizeChange}
      />

      {editingLocation && (
        <EditLocationDialog
          key={editingLocation.id}
          location={editingLocation}
          open={editOpen}
          onOpenChange={setEditOpen}
        />
      )}

      {deletingLocation && (
        <DeleteLocationDialog
          location={deletingLocation}
          open={deleteOpen}
          onOpenChange={setDeleteOpen}
        />
      )}
    </div>
  );
}

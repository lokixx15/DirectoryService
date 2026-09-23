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
import { useEffect, useMemo, useState } from "react";

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
import { DepartmentSelect } from "@/features/departments/ui/department-select/department-select";
import { Button } from "@/shared/components/ui/button";
import { useLocationFilters } from "@/features/locations/model/use-location-filters";
import { Skeleton } from "@/shared/components/ui/skeleton";
import { useLocationList } from "@/features/locations/model/use-location-list";
import { createLocationColumns } from "@/features/locations/model/location-columns";
import { CreateLocationDialog } from "@/features/locations/ui/create-location-dialog";
import { EditLocationDialog } from "@/features/locations/ui/edit-location-dialog";
import { DeleteDialog } from "@/shared/components/dialogs/delete-dialog";
import { RestoreDialog } from "@/shared/components/dialogs/restore-dialog";
import { useDeleteLocation } from "@/features/locations/model/use-delete-location";
import { useRestoreLocation } from "@/features/locations/model/use-restore-location";
import { Toggle } from "@/shared/components/ui/toggle";

export function LocationTableWidget() {
  const { pageSize, onPageSizeChange } = usePagination(10);
  const [search, setSearch] = useState("");
  const [sorting, setSorting] = useState<SortingState>([]);

  const {
    pageIndex,
    setPage,
    isActive,
    setIsActive,
    addedDepartmentIds,
    excludedDepartmentIds,
    setAddedDepartmentIdsHandler,
    setExcludedDepartmentIdsHandler,
  } = useLocationFilters();

  const [editingLocation, setEditingLocation] = useState<Location>();
  const [editOpen, setEditOpen] = useState<boolean>(false);

  const [deletingLocation, setDeletingLocation] = useState<Location>();
  const [deleteOpen, setDeleteOpen] = useState<boolean>(false);
  const { deleteLocation, isPending: isDeleteLocationPending } =
    useDeleteLocation();

  const [restoringLocation, setRestoringLocation] = useState<Location>();
  const [restoreOpen, setRestoreOpen] = useState<boolean>(false);
  const { restoreLocation, isPending: isRestoreLocationPending } =
    useRestoreLocation();

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
        (location) => {
          setRestoringLocation(location);
          setRestoreOpen(true);
        },
      ),
    [],
  );

  useEffect(() => {
    if (isActive === false) {
      setColumnVisibility({
        name: true,
        deletedAt: true,
        id: false,
        address: false,
        timezone: false,
        isActive: false,
        createdAt: false,
        updatedAt: false,
        actions: false,
        restoreAction: true,
      });
    } else {
      setColumnVisibility({
        id: false,
        name: true,
        address: true,
        timezone: true,
        isActive: true,
        createdAt: true,
        updatedAt: true,
        actions: true,
        deletedAt: false,
        restoreAction: false,
      });
    }
  }, [isActive]);

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
        <DepartmentSelect
          key={addedDepartmentIds.join(",") + excludedDepartmentIds.join(",")}
          addedDepartmentIds={addedDepartmentIds}
          onAddedDepartmentIdsChange={setAddedDepartmentIdsHandler}
          excludedDepartmentIds={excludedDepartmentIds}
          onExcludedDepartmentIdsChange={setExcludedDepartmentIdsHandler}
        />
        <Toggle
          size="sm"
          variant="outline"
          pressed={isActive}
          onPressedChange={setIsActive}
        >
          <span>{isActive ? "Active" : "Archive"}</span>
        </Toggle>
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
        <DeleteDialog
          entity={deletingLocation}
          open={deleteOpen}
          onOpenChange={setDeleteOpen}
          onDeleteEntity={deleteLocation}
          isPending={isDeleteLocationPending}
          name="location"
        />
      )}

      {restoringLocation && (
        <RestoreDialog
          entity={restoringLocation}
          open={restoreOpen}
          onOpenChange={setRestoreOpen}
          onRestoreEntity={restoreLocation}
          isPending={isRestoreLocationPending}
          name="location"
        />
      )}
    </div>
  );
}

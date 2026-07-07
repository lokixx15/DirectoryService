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
import { DepartmentMenu } from "@/features/departments";
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

export function LocationTableWidget() {
  const { page, pageSize, onPageIndexChange, onPageSizeChange } =
    usePagination(10);
  const [search, setSearch] = useState("");
  const [sorting, setSorting] = useState<SortingState>([]);
  const [isActive, setIsActive] = useState<boolean>();
  const [departmentIds, setDepartmentIds] = useState<string[]>([]);

  const [editingLocation, setEditingLocation] = useState<Location>();
  const [editOpen, setEditOpen] = useState<boolean>(false);

  const [deletingLocation, setDeletingLocation] = useState<Location>();
  const [deleteOpen, setDeleteOpen] = useState<boolean>(false);

  const { locations, totalCount, totalPages, isPending, errors, refetch } =
    useLocationList({
      page,
      pageSize,
      search,
      sorting,
      isActive,
      departmentIds,
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
        pageIndex: page,
        pageSize: pageSize,
      },
      columnVisibility,
      sorting,
    },
  });

  return (
    <div className="flex flex-col gap-2">
      <div className="flex flex-row items-center justify-between gap-2">
        <SearchBar onSearch={setSearch} />
        <div className="flex flex-row items-center gap-2">
          <ColumnsDropdown table={table} />
          <StatusFilter onIsActive={setIsActive} />
          <DepartmentMenu onDepartmentIdsChange={setDepartmentIds}>Related departments</DepartmentMenu>
          <CreateLocationDialog />
        </div>
      </div>

      {errors?.length ? (
        <ErrorCard errors={errors} refetch={refetch} />
      ) : isPending ? (
        <SkeletonTable />
      ) : (
        <div className="overflow-hidden rounded-md border">
          <Table>
            <TableHeader>
              {table.getHeaderGroups().map((headerGroup) => (
                <TableRow
                  key={headerGroup.id}
                  className="border-b-foreground/50"
                >
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
                    No results.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      )}

      <PaginationIconsOnly
        page={page}
        pageSize={pageSize}
        totalCount={totalCount}
        totalPages={totalPages}
        onPageIndexChange={onPageIndexChange}
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

"use client";

import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  OnChangeFn,
  SortingState,
  useReactTable,
  VisibilityState,
} from "@tanstack/react-table";

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/components/ui/table";
import { PaginationIconsOnly } from "@/shared/components/pagination/pagination-icons-only";
import { SearchBar } from "@/shared/components/search/search-bar";
import { memo, useState } from "react";

import { ColumnsDropdown } from "@/shared/components/dropdowns/columns-dropdown";
import { StatusFilter, CreateLocationDialog } from "@/features/locations";
import { DepartmentMenu } from "@/features/departments";

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[];
  data: TData[];
  page: number;
  pageSize: number;
  totalPages: number;
  onPageIndexChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
  sorting: SortingState;
  onSortingChange: OnChangeFn<SortingState>;
  onSearch: (value: string) => void;
  onIsActive: (value: boolean | undefined) => void;
  onDepartmentIdsChange: (departmentIds: string[]) => void;
}

const DataTableRaw = <TData, TValue>({
  columns,
  data,
  page,
  pageSize,
  totalPages,
  onPageIndexChange,
  onPageSizeChange,
  sorting,
  onSortingChange,
  onSearch,
  onIsActive,
  onDepartmentIdsChange,
}: DataTableProps<TData, TValue>) => {
  const [columnVisibility, setColumnVisibility] = useState<VisibilityState>({
    id: false,
  });
  // eslint-disable-next-line react-hooks/incompatible-library
  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
    manualPagination: true,
    manualSorting: true,
    enableSortingRemoval: false,
    onColumnVisibilityChange: setColumnVisibility,
    onSortingChange: onSortingChange,
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
        <SearchBar onSearch={onSearch} />
        <div className="flex flex-row items-center gap-2">
          <ColumnsDropdown table={table} />
          <StatusFilter onIsActive={onIsActive} />
          <DepartmentMenu onDepartmentIdsChange={onDepartmentIdsChange} />
          <CreateLocationDialog />
        </div>
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
                  colSpan={columns.length}
                  className="h-24 text-center"
                >
                  Нет результатов.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>

      <PaginationIconsOnly
        page={page}
        pageSize={pageSize}
        totalPages={totalPages}
        canPreviousPage={page > 0}
        canNextPage={page < totalPages - 1}
        onPageIndexChange={onPageIndexChange}
        onPageSizeChange={onPageSizeChange}
      />
    </div>
  );
};

export const DataTable = memo(DataTableRaw) as typeof DataTableRaw;

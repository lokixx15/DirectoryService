import { Location, formatAddress } from "@/entities/locations";
import { FormatDate } from "@/shared/lib/format-date";
import { Button } from "@/shared/components/ui/button";
import { ColumnDef } from "@tanstack/react-table";
import { ArrowUpDown, Pen, Trash } from "lucide-react";

export function createLocationColumns(
  onEdit: (location: Location) => void,
  onDelete: (location: Location) => void,
): ColumnDef<Location>[] {
  return [
    {
      accessorKey: "id",
      header: "ID",
    },
    {
      accessorKey: "name",
      header: ({ column }) => {
        return (
          <div
            onClick={column.getToggleSortingHandler()}
            className="flex items-center gap-2 cursor-pointer select-none hover:text-foreground"
          >
            Name
            <ArrowUpDown className="h-4 w-4" />
          </div>
        );
      },
    },
    {
      accessorKey: "address",
      header: "Address",
      cell: ({ row }) => formatAddress(row.original.address),
    },
    {
      accessorKey: "timezone",
      header: "Timezone",
    },
    {
      accessorKey: "isActive",
      header: "Active",
    },
    {
      accessorKey: "createdAt",
      header: ({ column }) => {
        return (
          <div
            onClick={column.getToggleSortingHandler()}
            className="flex items-center gap-2 cursor-pointer select-none hover:text-foreground"
          >
            Created
            <ArrowUpDown className="h-4 w-4" />
          </div>
        );
      },
      cell: ({ row }) => FormatDate(row.original.createdAt),
    },
    {
      accessorKey: "updatedAt",
      header: ({ column }) => {
        return (
          <div
            onClick={column.getToggleSortingHandler()}
            className="flex items-center gap-2 cursor-pointer select-none hover:text-foreground"
          >
            Updated
            <ArrowUpDown className="h-4 w-4" />
          </div>
        );
      },
      cell: ({ row }) => FormatDate(row.original.updatedAt),
    },
    {
      id: "actions",
      cell: ({ row }) => {
        const location = row.original;

        return (
          <div className="flex items-center justify-end gap-1">
            <Button
              variant="ghost"
              size="icon-sm"
              onClick={() => {
                onEdit(location);
              }}
            >
              <Pen className="h-4 w-4" />
            </Button>
            <Button
              variant="ghost"
              size="icon-sm"
              onClick={() => {
                onDelete(location);
              }}
            >
              <Trash className="h-4 w-4" />
            </Button>
          </div>
        );
      },
    },
  ];
}

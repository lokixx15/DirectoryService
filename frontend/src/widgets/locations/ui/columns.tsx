import { Location, formatAddress } from "@/entities/locations";
import { ColumnDef } from "@tanstack/react-table";
import { ArrowUpDown } from "lucide-react";

export const columns: ColumnDef<Location>[] = [
  {
    accessorKey: "id",
    header: "Id",
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
    header: "Is Active",
  },
  {
    accessorKey: "createdAt",
    header: ({ column }) => {
      return (
        <div
          onClick={column.getToggleSortingHandler()}
          className="flex items-center gap-2 cursor-pointer select-none hover:text-foreground"
        >
          Created At
          <ArrowUpDown className="h-4 w-4" />
        </div>
      );
    },
    cell: ({ row }) =>
      new Date(row.original.createdAt).toLocaleString("en-US", {
        timeZone: "UTC",
      }),
  },
  {
    accessorKey: "updatedAt",
    header: ({ column }) => {
      return (
        <div
          onClick={column.getToggleSortingHandler()}
          className="flex items-center gap-2 cursor-pointer select-none hover:text-foreground"
        >
          Updated At
          <ArrowUpDown className="h-4 w-4" />
        </div>
      );
    },
    cell: ({ row }) =>
      new Date(row.original.updatedAt).toLocaleString("en-US", {
        timeZone: "UTC",
      }),
  },
];

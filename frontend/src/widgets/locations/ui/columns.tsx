import { Location, formatAddress } from "@/entities/locations";
import { ColumnDef } from "@tanstack/react-table";
import { ArrowUpDown } from "lucide-react";

export const columns: ColumnDef<Location>[] = [
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
          Название
          <ArrowUpDown className="h-4 w-4" />
        </div>
      );
    },
  },
  {
    accessorKey: "address",
    header: "Адрес",
    cell: ({ row }) => formatAddress(row.original.address),
  },
  {
    accessorKey: "timezone",
    header: "Часовой пояс",
  },
  {
    accessorKey: "isActive",
    header: "Активна",
  },
  {
    accessorKey: "createdAt",
    header: ({ column }) => {
      return (
        <div
          onClick={column.getToggleSortingHandler()}
          className="flex items-center gap-2 cursor-pointer select-none hover:text-foreground"
        >
          Создана
          <ArrowUpDown className="h-4 w-4" />
        </div>
      );
    },
    cell: ({ row }) =>
      new Date(row.original.createdAt).toLocaleString("ru-RU", {
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
          Обновлена
          <ArrowUpDown className="h-4 w-4" />
        </div>
      );
    },
    cell: ({ row }) =>
      new Date(row.original.updatedAt).toLocaleString("ru-RU", {
        timeZone: "UTC",
      }),
  },
];

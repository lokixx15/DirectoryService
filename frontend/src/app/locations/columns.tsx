"use client";

import { Location } from "@/entities/locations/types";
import { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<Location>[] = [
  {
    accessorKey: "id",
    header: () => null,
    cell: () => null,
  },
  {
    accessorKey: "name",
    header: "Name",
  },
  {
    accessorKey: "address",
    header: "Address",
    cell: ({ row }) => {
      const address = row.original.address;
      const parts = [
        address.country,
        address.city,
        address.street,
        address.building,
        address.region,
        address.district,
        address.apartment,
      ].filter((part) => part && part.trim());

      return parts.join(", ");
    },
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
    header: "Created At",
    cell: ({ row }) =>
      new Date(row.original.createdAt).toLocaleString("en-US", {
        timeZone: "UTC",
      }),
  },
  {
    accessorKey: "updatedAt",
    header: "Updated At",
    cell: ({ row }) =>
      new Date(row.original.updatedAt).toLocaleString("en-US", {
        timeZone: "UTC",
      }),
  },
];

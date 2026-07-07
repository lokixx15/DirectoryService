"use client";

import { ReactNode } from "react";
import { SearchBar } from "@/shared/components/search/search-bar";
import { OrderFilter } from "@/shared/components/filters/order-filter";
import { StatusFilter } from "@/shared/components/filters/status-filter";
import { OrderState } from "@/shared/types/ordering";

interface DepartmentFiltersProps {
  onSearch: (value: string) => void;
  onOrder: (value: OrderState | undefined) => void;
  onStatus: (value: boolean | undefined) => void;
  actions?: ReactNode;
}

export function DepartmentFilters({
  onSearch,
  onOrder,
  onStatus,
  actions,
}: DepartmentFiltersProps) {
  return (
    <div className="flex flex-row items-center justify-between gap-2 w-full">
      <SearchBar onSearch={onSearch} />
      <div className="flex flex-row items-center gap-2">
        <OrderFilter onOrderChange={onOrder} />
        <StatusFilter onIsActive={onStatus} />
        {actions}
      </div>
    </div>
  );
}

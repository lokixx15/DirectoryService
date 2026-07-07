"use client";

import { useState } from "react";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
} from "@/shared/components/ui/select";
import { OrderBy, OrderDirection, OrderState } from "@/shared/types/ordering";
import { ArrowDown, ArrowUp } from "lucide-react";

interface OrderFilterProps {
  onOrderChange: (value: OrderState | undefined) => void;
}

const items = [
  { label: "Default", value: "none", icon: null },
  {
    label: "Name (A-Z)",
    value: "name:asc",
    icon: ArrowUp,
  },
  {
    label: "Name (Z-A)",
    value: "name:desc",
    icon: ArrowDown,
  },
  {
    label: "Created",
    value: "createdDate:asc",
    icon: ArrowUp,
  },
  {
    label: "Created",
    value: "createdDate:desc",
    icon: ArrowDown,
  },
  {
    label: "Updated",
    value: "updatedDate:asc",
    icon: ArrowUp,
  },
  {
    label: "Updated",
    value: "updatedDate:desc",
    icon: ArrowDown,
  },
];

export function OrderFilter({ onOrderChange }: OrderFilterProps) {
  const [selectedValue, setSelectedValue] = useState(items[0].value);

  const handleOrderChange = (value: string) => {
    setSelectedValue(value);

    if (value === "none") {
      onOrderChange(undefined);
      return;
    }

    const [orderBy, orderDirection] = value.split(":") as [
      OrderBy,
      OrderDirection,
    ];
    onOrderChange({ orderBy, orderDirection });
  };

  const selectedItem = items.find((item) => item.value === selectedValue);
  const hasIcon = !!selectedItem?.icon;

  return (
    <Select defaultValue={items[0].value} onValueChange={handleOrderChange}>
      <SelectTrigger
        hideChevron={hasIcon}
        className="w-full max-w-48 h-8 border-border font-medium gap-1"
      >
        <span>{selectedItem?.label || items[0].label}</span>
        {hasIcon && selectedItem.icon && (
          <selectedItem.icon className="h-4 w-4 opacity-50 shrink-0" />
        )}
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          {items.map((item) => (
            <SelectItem key={item.value} value={item.value} hideIndicator>
              <div className="flex items-center gap-1">
                {item.label}
                {item.icon && (
                  <item.icon className="h-4 w-4 text-muted-foreground" />
                )}
              </div>
            </SelectItem>
          ))}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
}

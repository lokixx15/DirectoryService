"use client";

import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select";

interface StatusFilterProps {
  onIsActive: (value: boolean | undefined) => void;
}

const items = [
  { label: "Все", value: "all" },
  { label: "Активные", value: "true" },
  { label: "Неактивные", value: "false" },
];

export function StatusFilter({ onIsActive }: StatusFilterProps) {
  const handleStatusChange = (value: string) => {
    if (value === "true") onIsActive(true);
    else if (value === "false") onIsActive(false);
    else onIsActive(undefined);
  };

  return (
    <Select defaultValue="all" onValueChange={handleStatusChange}>
      <SelectTrigger className="w-full max-w-48 h-8 border-border">
        <SelectValue placeholder={items[0].label} />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          {items.map((item) => (
            <SelectItem key={item.value} value={item.value}>
              {item.label}
            </SelectItem>
          ))}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
}

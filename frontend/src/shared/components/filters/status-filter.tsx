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
  { label: "All", value: "all" },
  { label: "Active", value: "true" },
  { label: "Inactive", value: "false" },
];

export function StatusFilter({ onIsActive }: StatusFilterProps) {
  const handleStatusChange = (value: string) => {
    if (value === "true") onIsActive(true);
    else if (value === "false") onIsActive(false);
    else onIsActive(undefined);
  };

  return (
    <Select defaultValue={items[0].value} onValueChange={handleStatusChange}>
      <SelectTrigger className="w-fit h-9 border-border font-medium">
        <SelectValue placeholder={items[0].label} />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          {items.map((item) => (
            <SelectItem key={item.value} value={item.value} className="bold">
              {item.label}
            </SelectItem>
          ))}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
}

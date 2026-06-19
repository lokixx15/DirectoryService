"use client";

import { useEffect, useState } from "react";
import { useDebounce } from "@/shared/hooks/use-debounce";
import { Input } from "@/shared/components/ui/input";

interface SearchBarProps {
  onSearch: (value: string) => void;
}

export function SearchBar({ onSearch }: SearchBarProps) {
  const [value, setValue] = useState("");
  const debouncedValue = useDebounce(value, 300);

  useEffect(() => {
    onSearch(debouncedValue);
  }, [debouncedValue, onSearch]);

  return (
    <Input
      type="text"
      value={value}
      onChange={(e) => setValue(e.target.value)}
      placeholder="Search..."
      className="w-[25%] min-w-62.5 rounded-md border-border"
    />
  );
};

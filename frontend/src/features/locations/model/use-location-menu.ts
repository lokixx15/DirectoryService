import { LocationSummary } from "@/entities/locations/types";
import { useState } from "react";

interface UseLocationMenuReturn {
  open: boolean;
  selectedLocations: LocationSummary[];
  appliedLocations: LocationSummary[];
  toggleLocation: (Location: LocationSummary) => void;
  removeLocation: (id: string) => void;
  applySelectedLocations: () => void;
  onOpenDialogChange: (value: boolean) => void;
  clearSelectedLocations: () => void;
}

export function useLocationMenu(
  onChange: (ids: string[]) => void,
): UseLocationMenuReturn {
  const [open, setOpen] = useState<boolean>(false);
  const [selected, setSelected] = useState<LocationSummary[]>([]);
  const [applied, setApplied] = useState<LocationSummary[]>([]);

  const toggle = (location: LocationSummary) => {
    setSelected((prev) =>
      prev.some((d) => d.id === location.id)
        ? prev.filter((d) => d.id !== location.id)
        : [...prev, location],
    );
  };

  const remove = (id: string) => {
    setSelected((prev) => prev.filter((d) => d.id !== id));
  };

  const applySelected = () => {
    onChange(selected.map((sD) => sD.id));
    setApplied(selected);
    setOpen(false);
  };

  const onOpenDialogChange = (value: boolean) => {
    setOpen(value);
    setSelected(applied);
  };

  const clearSelected = () => setSelected([]);

  return {
    open: open,
    selectedLocations: selected,
    appliedLocations: applied,
    toggleLocation: toggle,
    removeLocation: remove,
    applySelectedLocations: applySelected,
    onOpenDialogChange: onOpenDialogChange,
    clearSelectedLocations: clearSelected,
  };
}

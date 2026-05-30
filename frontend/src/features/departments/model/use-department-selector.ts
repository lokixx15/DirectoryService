import { DepartmentSummary } from "@/entities/departments";
import { useState } from "react";

interface UseDepartmentSelectorReturn {
  open: boolean;
  selectedDepartments: DepartmentSummary[];
  appliedDepartments: DepartmentSummary[];
  toggleDepartment: (department: DepartmentSummary) => void;
  removeDepartment: (id: string) => void;
  applySelectedDepartments: () => void;
  onOpenDialogChange: (value: boolean) => void;
  clearSelectedDepartments: () => void;
}

export function useDepartmentSelector(
  onChange: (ids: string[]) => void,
): UseDepartmentSelectorReturn {
  const [open, setOpen] = useState<boolean>(false);
  const [selected, setSelected] = useState<DepartmentSummary[]>([]);
  const [applied, setApplied] = useState<DepartmentSummary[]>([]);

  const toggle = (department: DepartmentSummary) => {
    setSelected((prev) =>
      prev.some((d) => d.id === department.id)
        ? prev.filter((d) => d.id !== department.id)
        : [...prev, department],
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
    selectedDepartments: selected,
    appliedDepartments: applied,
    toggleDepartment: toggle,
    removeDepartment: remove,
    applySelectedDepartments: applySelected,
    onOpenDialogChange: onOpenDialogChange,
    clearSelectedDepartments: clearSelected,
  };
}

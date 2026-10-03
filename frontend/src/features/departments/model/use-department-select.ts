import { useState } from "react";

interface UseDepartmentMultiSelectReturn {
  open: boolean;
  onOpenDropdown: (open: boolean) => void;
  selectedAddedDepartments: string[];
  addedDepartments: string[];
  addDepartment: (department: string) => void;
  removeAddedDepartment: (id: string) => void;
  applySelectedAddedDepartments: () => void;
  clearSelectedAddedDepartments: () => void;
}

interface UseDepartmentMultiSelectProps {
  onAddedChange: (ids: string[]) => void;
  initialAdded?: string[];
  initialExcluded?: string[];
}

export function useDepartmentMultiSelect({
  onAddedChange,
  initialAdded = [],
}: UseDepartmentMultiSelectProps): UseDepartmentMultiSelectReturn {
  const [open, setOpen] = useState(false);

  const [selectedAddedDepartments, setSelectedAddedDepartments] =
    useState<string[]>(initialAdded);

  const [addedDepartments, setAddedDepartments] =
    useState<string[]>(initialAdded);

  const addDepartments = (departmentId: string) => {
    if (addedDepartments.some((id) => id === departmentId)) {
      return;
    }

    setSelectedAddedDepartments((prev) =>
      prev.some((id) => id === departmentId) ? prev : [...prev, departmentId],
    );
  };

  const removeAdded = (departmentId: string) => {
    setSelectedAddedDepartments((prev) =>
      prev.filter((id) => id !== departmentId),
    );
  };

  const applyAdded = () => {
    onAddedChange(selectedAddedDepartments.map((id) => id));
    setAddedDepartments(selectedAddedDepartments);
  };

  const clearAdded = () => setSelectedAddedDepartments([]);

  return {
    open,
    onOpenDropdown: setOpen,
    selectedAddedDepartments,
    addedDepartments,
    addDepartment: addDepartments,
    removeAddedDepartment: removeAdded,
    applySelectedAddedDepartments: applyAdded,
    clearSelectedAddedDepartments: clearAdded,
  };
}

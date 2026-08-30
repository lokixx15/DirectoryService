import { useEffect, useState } from "react";

interface UseDepartmentselectReturn {
  open: boolean;
  onOpenDropdown: (open: boolean) => void;
  selectedAddedDepartments: string[];
  addedDepartments: string[];
  selectedExcludedDepartments: string[];
  excludedDepartments: string[];
  addDepartment: (department: string) => void;
  removeAddedDepartment: (id: string) => void;
  applySelectedAddedDepartments: () => void;
  clearSelectedAddedDepartments: () => void;
  addExcludedDepartment: (department: string) => void;
  removeExcludedDepartment: (id: string) => void;
  applySelectedExcludedDepartments: () => void;
  clearExcludedDepartments: () => void;
}

interface UseDepartmentSelectProps {
  onAddedChange: (ids: string[]) => void;
  onExcludeChange: (ids: string[]) => void;
  initialAdded?: string[];
  initialExcluded?: string[];
}

export function useDepartmentSelect({
  onAddedChange,
  onExcludeChange,
  initialAdded = [],
  initialExcluded = [],
}: UseDepartmentSelectProps): UseDepartmentselectReturn {
  const [open, setOpen] = useState(false);

  const [selectedAddedDepartments, setSelectedAddedDepartments] =
    useState<string[]>(initialAdded);

  const [addedDepartments, setAddedDepartments] =
    useState<string[]>(initialAdded);

  const [selectedExcludedDepartments, setSelectedExcludedDepartments] =
    useState<string[]>(initialExcluded);

  const [excludedDepartments, setExcludedDepartments] =
    useState<string[]>(initialExcluded);

  const addDepartments = (departmentId: string) => {
    if (
      addedDepartments.some((id) => id === departmentId) ||
      excludedDepartments.some((id) => id === departmentId)
    ) {
      return;
    }

    if (selectedExcludedDepartments.some((id) => id === departmentId)) {
      setSelectedExcludedDepartments((prev) =>
        prev.filter((id) => id !== departmentId),
      );
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

  const addExcluded = (departmentId: string) => {
    if (
      addedDepartments.some((id) => id === departmentId) ||
      excludedDepartments.some((id) => id === departmentId)
    ) {
      return;
    }

    if (selectedAddedDepartments.some((id) => id === departmentId)) {
      setSelectedAddedDepartments((prev) =>
        prev.filter((id) => id !== departmentId),
      );
    }

    setSelectedExcludedDepartments((prev) =>
      prev.some((id) => id === departmentId) ? prev : [...prev, departmentId],
    );
  };

  const removeExcluded = (departmentId: string) => {
    setSelectedExcludedDepartments((prev) =>
      prev.filter((id) => id !== departmentId),
    );
  };

  const applyExcluded = () => {
    onExcludeChange(selectedExcludedDepartments.map((id) => id));
    setExcludedDepartments(selectedExcludedDepartments);
    setOpen(false);
  };

  const clearExcluded = () => setSelectedExcludedDepartments([]);

  return {
    open,
    onOpenDropdown: setOpen,
    selectedAddedDepartments,
    addedDepartments,
    selectedExcludedDepartments,
    excludedDepartments,
    addDepartment: addDepartments,
    removeAddedDepartment: removeAdded,
    applySelectedAddedDepartments: applyAdded,
    clearSelectedAddedDepartments: clearAdded,
    addExcludedDepartment: addExcluded,
    removeExcludedDepartment: removeExcluded,
    applySelectedExcludedDepartments: applyExcluded,
    clearExcludedDepartments: clearExcluded,
  };
}

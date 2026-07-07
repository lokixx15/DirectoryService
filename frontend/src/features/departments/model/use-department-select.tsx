import { DepartmentStandard } from "@/entities/departments/types";
import { useState } from "react";

interface UseDepartmentselectReturn {
  selectedAddedDepartments: DepartmentStandard[];
  addedDepartments: DepartmentStandard[];
  selectedExcludedDepartments: DepartmentStandard[];
  excludedDepartments: DepartmentStandard[];

  addDepartment: (department: DepartmentStandard) => void;
  removeAddedDepartment: (id: string) => void;
  applySelectedAddedDepartments: () => void;
  clearSelectedAddedDepartments: () => void;

  addExcludedDepartment: (department: DepartmentStandard) => void;
  removeExcludedDepartment: (id: string) => void;
  applySelectedExcludedDepartments: () => void;
  clearExcludedDepartments: () => void;
}

interface UseDepartmentSelectProps {
  onAddedChange: (ids: string[]) => void;
  onExcludeChange: (ids: string[]) => void;
}

export function useDepartmentSelect({
  onAddedChange,
  onExcludeChange,
}: UseDepartmentSelectProps): UseDepartmentselectReturn {
  const [selectedAddedDepartments, setSelectedAddedDepartments] = useState<
    DepartmentStandard[]
  >([]);
  const [addedDepartments, setAddedDepartments] = useState<
    DepartmentStandard[]
  >([]);

  const [selectedExcludedDepartments, setSelectedExcludedDepartments] =
    useState<DepartmentStandard[]>([]);
  const [excludedDepartments, setExcludedDepartments] = useState<
    DepartmentStandard[]
  >([]);

  const addDepartments = (department: DepartmentStandard) => {
    if (
      addedDepartments.some((d) => d.id === department.id) ||
      excludedDepartments.some((d) => d.id === department.id)
    ) {
      return;
    }

    if (selectedExcludedDepartments.some((d) => d.id === department.id)) {
      setSelectedExcludedDepartments((prev) =>
        prev.filter((d) => d.id !== department.id),
      );
    }

    setSelectedAddedDepartments((prev) =>
      prev.some((d) => d.id === department.id) ? prev : [...prev, department],
    );
  };

  const removeAdded = (id: string) => {
    setSelectedAddedDepartments((prev) => prev.filter((d) => d.id !== id));
  };

  const applyAdded = () => {
    onAddedChange(selectedAddedDepartments.map((sD) => sD.id));
    setAddedDepartments(selectedAddedDepartments);
  };

  const clearAdded = () => setSelectedAddedDepartments([]);

  const addExcluded = (department: DepartmentStandard) => {
    if (
      addedDepartments.some((d) => d.id === department.id) ||
      excludedDepartments.some((d) => d.id === department.id)
    ) {
      return;
    }

    if (selectedAddedDepartments.some((d) => d.id === department.id)) {
      setSelectedAddedDepartments((prev) =>
        prev.filter((d) => d.id !== department.id),
      );
    }

    setSelectedExcludedDepartments((prev) =>
      prev.some((d) => d.id === department.id) ? prev : [...prev, department],
    );
  };

  const removeExcluded = (id: string) => {
    setSelectedExcludedDepartments((prev) => prev.filter((d) => d.id !== id));
  };

  const applyExcluded = () => {
    onExcludeChange(selectedExcludedDepartments.map((sD) => sD.id));
    setExcludedDepartments(selectedExcludedDepartments);
  };

  const clearExcluded = () => setSelectedExcludedDepartments([]);

  return {
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

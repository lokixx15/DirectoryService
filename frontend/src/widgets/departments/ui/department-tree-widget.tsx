"use client";

import { useState } from "react";
import { DepartmentSelect } from "@/features/departments/ui/department-select/department-select";
import { LocationMenu } from "@/features/locations/ui/location-menu";

export function DepartmentTreeWidget() {
  const [departmentIds, setDepartmentIds] = useState<string[]>([]);
  const [excludeDepartmentIds, setExcludeDepartmentIds] = useState<string[]>(
    [],
  );
  const [locationIds, setLocationIds] = useState<string[]>([]);

  return (
    <DepartmentSelect
      addedDepartmentIds={departmentIds}
      onAddedDepartmentIdsChange={setDepartmentIds}
      excludedDepartmentIds={excludeDepartmentIds}
      onExcludedDepartmentIdsChange={setExcludeDepartmentIds}
      locationIds={locationIds}
      filterActions={
        <LocationMenu onLocationIdsChange={setLocationIds}>
          Related locations
        </LocationMenu>
      }
    />
  );
}

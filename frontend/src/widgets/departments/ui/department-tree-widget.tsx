"use client";

import { useState } from "react";
import { DepartmentSelect } from "@/features/departments/ui/department-select/department-select";
import { LocationMenu } from "@/features/locations/ui/location-menu";

export function DepartmentTreeWidget() {
  const [locationIds, setLocationIds] = useState<string[]>([]);

  return (
    <DepartmentSelect
      locationIds={locationIds}
      filterActions={
        <LocationMenu onLocationIdsChange={setLocationIds}>
          Related locations
        </LocationMenu>
      }
    />
  );
}

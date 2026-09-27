import { DepartmentTreeWidget } from "@/widgets/departments/ui/department-tree-widget";
import { Suspense } from "react";

export default function Home() {
  return (
    <Suspense>
      <DepartmentTreeWidget />
    </Suspense>
  );
}

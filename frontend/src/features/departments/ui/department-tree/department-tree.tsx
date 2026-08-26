import { Department } from "@/entities/departments/types";
import { DepartmentTreeNode } from "./department-tree-node";

interface DepartmentTreeProps {
  departments: Department[];
  selectedId?: string;
  onSelectedId: (id: string) => void;
}

export function DepartmentTree({
  departments,
  selectedId,
  onSelectedId,
}: DepartmentTreeProps) {
  const rootDepartments = departments.filter((d) => d.parentId === null);

  const childrenByParentId = new Map<string, Department[]>();

  for (const d of departments) {
    if (d.parentId) {
      const children = childrenByParentId.get(d.parentId) ?? [];
      children.push(d);
      childrenByParentId.set(d.parentId, children);
    }
  }

  return (
    <div className="flex flex-col gap-1">
      {rootDepartments.map((item) => (
        <DepartmentTreeNode
          key={item.id}
          department={item}
          prefetchedChildren={childrenByParentId.get(item.id) ?? []}
          selectedId={selectedId}
          onSelectId={onSelectedId}
        />
      ))}
    </div>
  );
}

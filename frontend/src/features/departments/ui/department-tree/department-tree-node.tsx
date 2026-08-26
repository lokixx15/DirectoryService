import { useMemo, useState } from "react";
import { ChevronRightIcon } from "lucide-react";
import { Button } from "@/shared/components/ui/button";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/shared/components/ui/collapsible";
import { Department } from "@/entities/departments/types";
import { DepartmentTreeCard } from "./department-tree-card";
import { useChildrenDepartments } from "../../model/use-children-departments";
import { LoadMoreButton } from "@/shared/components/pagination/load-more-button";
import { usePagination } from "@/shared/hooks/use-pagination";
import { ErrorCard } from "@/shared/components/errors/error-card";

interface DepartmentTreeNodeProps {
  department: Department;
  prefetchedChildren?: Department[];
  selectedId?: string;
  onSelectId: (id: string) => void;
}

export function DepartmentTreeNode({
  department,
  prefetchedChildren = [],
  selectedId,
  onSelectId,
}: DepartmentTreeNodeProps) {
  const { page, pageSize, onPageSizeChange } = usePagination(3);
  const [isOpen, setIsOpen] = useState(false);

  const {
    nestedChildrenDepartments,
    totalCount,
    isError,
    errors,
    isFetching,
    refetch,
  } = useChildrenDepartments({
    page: page,
    size: pageSize,
    parentId: department.id,
    enabled: isOpen,
  });

  const displayChildren = useMemo(() => {
    if (!nestedChildrenDepartments) return prefetchedChildren;

    const prefetchedIds = new Set(prefetchedChildren.map((c) => c.id));
    const merged = [...prefetchedChildren];

    for (const apiChild of nestedChildrenDepartments) {
      if (!prefetchedIds.has(apiChild.id)) {
        merged.push(apiChild);
      }
    }

    return merged;
  }, [nestedChildrenDepartments, prefetchedChildren]);

  const hasChildren = department.hasMoreChildren;

  const isSelected = selectedId === department.id;
  const handleSelect = () => onSelectId(department.id);

  if (isError || errors) {
    return <ErrorCard errors={errors ?? []} refetch={refetch} />;
  }

  if (!hasChildren) {
    return (
      <DepartmentTreeCard
        department={department}
        isSelected={isSelected}
        onClick={handleSelect}
      />
    );
  }

  return (
    <Collapsible open={isOpen} onOpenChange={setIsOpen}>
      <DepartmentTreeCard
        department={department}
        isSelected={isSelected}
        onClick={handleSelect}
      >
        <CollapsibleTrigger asChild>
          <Button variant="ghost" size="icon" className="group size-7 shrink-0">
            <ChevronRightIcon className="size-4 transition-transform group-data-[state=open]:rotate-90" />
          </Button>
        </CollapsibleTrigger>
      </DepartmentTreeCard>

      <CollapsibleContent className="mt-1 flex flex-col">
        <div className="ml-5 pl-3 border-l-2 border-muted flex flex-col gap-1">
          {displayChildren.map((child) => (
            <DepartmentTreeNode
              key={child.id}
              department={child}
              selectedId={selectedId}
              onSelectId={onSelectId}
            />
          ))}
        </div>

        <div className="mx-auto mb-2">
          <LoadMoreButton
            pageSize={pageSize}
            totalElements={totalCount ?? 0}
            onPageSizeChange={onPageSizeChange}
            loading={isFetching}
            size="xs"
          >
            Load Children
          </LoadMoreButton>
        </div>
      </CollapsibleContent>
    </Collapsible>
  );
}

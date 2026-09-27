import { Department } from "@/entities/departments/types";
import { ReactNode, useState } from "react";
import { useDepartmentRestore } from "../../model/use-department-restore";
import { useDepartmentDelete } from "../../model/use-department-delete";
import { RestoreDialog } from "@/shared/components/dialogs/restore-dialog";
import { DeleteDialog } from "@/shared/components/dialogs/delete-dialog";
import { DepartmentTreeActiveCard } from "./department-tree-active-card";
import { DepartmentTreeInactiveCard } from "./department-tree-inactive-card";

interface DepartmentTreeCardProps {
  department: Department;
  children?: ReactNode;
  isSelected?: boolean;
  onClick?: () => void;
}

export function DepartmentTreeCard({
  department,
  children,
  isSelected,
  onClick,
}: DepartmentTreeCardProps) {
  const { restoreDepartment, isPending: isRestoreDepartmentPending } =
    useDepartmentRestore();
  const { deleteDepartment, isPending: isDeleteDepartmentPending } =
    useDepartmentDelete();

  const [restoreOpen, setRestoreOpen] = useState<boolean>(false);
  const [deleteOpen, setDeleteOpen] = useState<boolean>(false);

  const [restoringDepartment, setRestoringDepartment] = useState<Department>();
  const [deletingDepartment, setDeletingDepartment] = useState<Department>();

  const handleActiveSwitch = (nextActiveState: boolean) => {
    if (nextActiveState) {
      setRestoreOpen(true);
      setRestoringDepartment(department);
    } else {
      setDeleteOpen(true);
      setDeletingDepartment(department);
    }
  };

  return (
    <div>
      {department.isActive ? (
        <DepartmentTreeActiveCard
          department={department}
          isSelected={isSelected}
          onClick={onClick}
          onIsActiveChange={handleActiveSwitch}
        >
          {children}
        </DepartmentTreeActiveCard>
      ) : (
        <DepartmentTreeInactiveCard
          department={department}
          onClick={onClick}
          onIsActiveChange={handleActiveSwitch}
        >
          {children}
        </DepartmentTreeInactiveCard>
      )}

      {deletingDepartment && (
        <DeleteDialog
          entity={deletingDepartment}
          open={deleteOpen}
          onOpenChange={setDeleteOpen}
          onDeleteEntity={deleteDepartment}
          isPending={isDeleteDepartmentPending}
          name="department"
        />
      )}

      {restoringDepartment && (
        <RestoreDialog
          entity={restoringDepartment}
          open={restoreOpen}
          onOpenChange={setRestoreOpen}
          onRestoreEntity={restoreDepartment}
          isPending={isRestoreDepartmentPending}
          name="department"
        />
      )}
    </div>
  );
}

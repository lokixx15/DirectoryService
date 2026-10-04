"use client";

import { Department, DepartmentSummary } from "@/entities/departments/types";
import { Button } from "@/shared/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
  DialogClose,
} from "@/shared/components/ui/dialog";
import { useState } from "react";
import { toast } from "sonner";
import { isEnvelopeError } from "@/shared/api/errors";
import { DepartmentSingleSelect } from "./department-select/department-single-select";

interface UpdateParentDialogProps {
  department: Department;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onUpdateParent: (
    { id, parentId }: { id: string; parentId?: string },
    options?: {
      onSuccess?: () => void;
      onError?: (errors: unknown) => void;
    },
  ) => void;
  isPending: boolean;
}

export function UpdateParentDialog({
  department,
  open,
  onOpenChange,
  onUpdateParent,
  isPending,
}: UpdateParentDialogProps) {
  const [newParent, setNewParent] = useState<DepartmentSummary>();
  const [isRootSelected, setIsRootSelected] = useState<boolean>(false);

  const updateParent = async () => {
    await onUpdateParent(
      { id: department.id, parentId: newParent?.id },
      {
        onSuccess: () => {
          toast.success(`${department.name} parent updated successfully`);
          onOpenChange(false);
        },
        onError: (errors) => {
          if (isEnvelopeError(errors)) {
            errors.apiErrors.forEach((error) => {
              toast.error(error.message);
            });
          } else {
            toast.error(`The ${department.name} failed to update parent`);
          }
        },
      },
    );
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="">
        <DialogHeader>
          <DialogTitle>Update parent</DialogTitle>
          <DialogDescription>
            Are you sure you want to update parent for &quot;{department.name}
            &quot;?
          </DialogDescription>
          <DepartmentSingleSelect
            excludedId={department.id}
            department={newParent}
            isRootSelected={isRootSelected}
            onDepartmentChange={(dept) => {
              if (!dept) {
                setIsRootSelected(true);
                setNewParent(undefined);
              } else {
                setIsRootSelected(false);
                setNewParent(dept);
              }
            }}
          />
        </DialogHeader>
        <DialogFooter>
          <DialogClose asChild>
            <Button variant="outline">Cancel</Button>
          </DialogClose>
          <div>
            <Button onClick={updateParent} disabled={isPending}>
              Apply
            </Button>
          </div>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

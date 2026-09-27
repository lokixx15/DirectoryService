"use client";

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
import { Location } from "@/entities/locations";
import { toast } from "sonner";
import { isEnvelopeError } from "@/shared/api/errors";
import { Department } from "@/entities/departments/types";
import { Position } from "@/entities/positions/types";

interface DeleteDialogProps {
  entity: Location | Department | Position | null | undefined;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onDeleteEntity: (
    id: string,
    options?: {
      onSuccess?: () => void;
      onError?: (errors: unknown) => void;
    },
  ) => void;
  isPending: boolean;
  name: string;
}

export function DeleteDialog({
  entity,
  open,
  onOpenChange,
  onDeleteEntity,
  isPending,
  name,
}: DeleteDialogProps) {
  if (!entity) return null;

  const onDelete = async () => {
    await onDeleteEntity(entity.id, {
      onSuccess: () => {
        toast.success(
          `${name.charAt(0).toUpperCase() + name.slice(1)} deleted successfully`,
        );
        onOpenChange(false);
      },
      onError: (errors) => {
        if (isEnvelopeError(errors)) {
          errors.apiErrors.forEach((error) => {
            toast.error(error.message);
          });
        } else {
          toast.error(`Failed to delete ${name}`);
        }
      },
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-sm">
        <DialogHeader>
          <DialogTitle>Delete {name}</DialogTitle>
          <DialogDescription>
            Are you sure you want to delete &quot;{entity.name}&quot;?
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <DialogClose asChild>
            <Button variant="outline">Cancel</Button>
          </DialogClose>
          <Button variant="destructive" disabled={isPending} onClick={onDelete}>
            Delete
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

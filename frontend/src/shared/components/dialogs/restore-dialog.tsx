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

interface RestoreDialogProps {
  entity: Location | Department | Position | null | undefined;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onRestoreEntity: (
    id: string,
    options?: {
      onSuccess?: () => void;
      onError?: (errors: unknown) => void;
    },
  ) => void;
  isPending: boolean;
  name: string;
}

export function RestoreDialog({
  entity,
  open,
  onOpenChange,
  onRestoreEntity,
  isPending,
  name,
}: RestoreDialogProps) {
  if (!entity) return;

  const onRestore = async () => {
    await onRestoreEntity(entity.id, {
      onSuccess: () => {
        toast.success(
          `${name.charAt(0).toUpperCase() + name.slice(1)} restored successfully`,
        );
        onOpenChange(false);
      },
      onError: (errors) => {
        if (isEnvelopeError(errors)) {
          errors.apiErrors.forEach((error) => {
            toast.error(error.message);
          });
        } else {
          toast.error(`Failed to restore ${name}`);
        }
      },
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-sm">
        <DialogHeader>
          <DialogTitle>Restore {name}</DialogTitle>
          <DialogDescription>
            Are you sure you want to restore &quot;{entity.name}&quot;?
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <DialogClose asChild>
            <Button variant="outline">Cancel</Button>
          </DialogClose>
          <Button variant="creative" disabled={isPending} onClick={onRestore}>
            Restore
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

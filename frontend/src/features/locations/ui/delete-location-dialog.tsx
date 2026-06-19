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
import { useDeleteLocation } from "../model/use-delete-location";
import { Location } from "@/entities/locations";
import { toast } from "sonner";
import { isEnvelopeError } from "@/shared/api/errors";

interface DeleteLocationDialogProps {
  location: Location | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function DeleteLocationDialog({
  location,
  open,
  onOpenChange,
}: DeleteLocationDialogProps) {
  const { deleteLocation, isPending } = useDeleteLocation();

  const onDelete = async () => {
    if (!location) return;

    await deleteLocation(location.id, {
      onSuccess: () => {
        toast.success("Location deleted successfully");
        onOpenChange(false);
      },
      onError: (errors) => {
        if (isEnvelopeError(errors)) {
          errors.apiErrors.forEach((error) => {
            toast.error(error.message);
          });
        } else {
          toast.error("Failed to delete location");
        }
      },
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-sm">
        <DialogHeader>
          <DialogTitle>Delete location</DialogTitle>
          <DialogDescription>
            Are you sure you want to delete &quot;{location?.name}&quot;? This
            action cannot be undone.
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

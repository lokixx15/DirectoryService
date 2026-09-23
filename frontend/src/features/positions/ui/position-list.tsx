import { Position } from "@/entities/positions/types";
import { Spinner } from "@/shared/components/ui/spinner";
import { cn } from "@/shared/lib/utils";
import { RefCallback, useCallback, useState } from "react";
import { usePositionRestore } from "../model/use-position-restore";
import { usePositionDelete } from "../model/use-position-delete";
import { ActivePositionCard } from "./active-position-card";
import { InactivePositionCard } from "./inactive-position-card";
import { RestoreDialog } from "@/shared/components/dialogs/restore-dialog";
import { DeleteDialog } from "@/shared/components/dialogs/delete-dialog";

interface PositionListProps {
  positions?: Position[];
  isFetchingNextPage: boolean;
  hasNextPage: boolean;
  fetchNextPage: () => void;
  layoutClassName?: string;
}

export function PositionList({
  positions,
  isFetchingNextPage,
  hasNextPage,
  fetchNextPage,
  layoutClassName,
}: PositionListProps) {
  const cursorRef: RefCallback<HTMLDivElement> = useCallback(
    (el) => {
      const observer = new IntersectionObserver(
        (entries) => {
          if (entries[0].isIntersecting && hasNextPage && !isFetchingNextPage) {
            fetchNextPage();
          }
        },
        {
          threshold: 0.5,
        },
      );

      if (el) {
        observer.observe(el);

        return () => observer.disconnect();
      }
    },
    [fetchNextPage, hasNextPage, isFetchingNextPage],
  );

  const { restorePosition, isPending: isRestorePositionPending } =
    usePositionRestore();
  const { deletePosition, isPending: isDeletePositionPending } =
    usePositionDelete();

  const [restoreOpen, setRestoreOpen] = useState<boolean>(false);
  const [deleteOpen, setDeleteOpen] = useState<boolean>(false);

  const [restoringPosition, setRestoringPosition] = useState<Position>();
  const [deletingPosition, setDeletingPosition] = useState<Position>();

  return (
    <div className="flex flex-col gap-5">
      <div
        className={cn(
          layoutClassName ??
            "grid sm:grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3",
        )}
      >
        {positions?.map((p) => {
          const handleActiveSwitch = (nextActiveState: boolean) => {
            if (nextActiveState) {
              setRestoreOpen(true);
              setRestoringPosition(p);
            } else {
              setDeleteOpen(true);
              setDeletingPosition(p);
            }
          };

          return p.isActive ? (
            <ActivePositionCard
              key={p.id}
              position={p}
              handleActiveSwitch={handleActiveSwitch}
            />
          ) : (
            <InactivePositionCard
              key={p.id}
              position={p}
              handleActiveSwitch={handleActiveSwitch}
            />
          );
        })}
      </div>

      {deletingPosition && (
        <DeleteDialog
          entity={deletingPosition}
          open={deleteOpen}
          onOpenChange={setDeleteOpen}
          onDeleteEntity={deletePosition}
          isPending={isDeletePositionPending}
          name="position"
        />
      )}

      {restoringPosition && (
        <RestoreDialog
          entity={restoringPosition}
          open={restoreOpen}
          onOpenChange={setRestoreOpen}
          onRestoreEntity={restorePosition}
          isPending={isRestorePositionPending}
          name="position"
        />
      )}

      <div ref={cursorRef} className="flex justify-center py-4">
        {isFetchingNextPage && <Spinner />}
      </div>
    </div>
  );
}

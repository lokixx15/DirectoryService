"use client";

import { AlertTriangle, RotateCcw } from "lucide-react";
import { Button } from "@/shared/components/ui/button";

interface ErrorLabelProps {
  refetch?: () => void;
}

export function ErrorLabel({ refetch }: ErrorLabelProps) {
  return (
    <div className="flex items-center gap-2 rounded-lg border border-destructive/30 bg-destructive/5 p-3 text-sm m-2">
      <AlertTriangle className="h-4 w-4 shrink-0 text-destructive" />
      <span className="text-muted-foreground">
        An error occurred while loading
      </span>
      {refetch && (
        <Button
          variant="secondary"
          size="xs"
          onClick={refetch}
          className="ml-auto"
        >
          <RotateCcw className="h-3 w-3" />
          Retry
        </Button>
      )}
    </div>
  );
}

"use client";

import { useState } from "react";
import {
  ChevronDown,
  ChevronUp,
  ChevronLeft,
  ChevronRight,
  AlertTriangle,
  Ban,
  FileX,
  ServerCrash,
  HelpCircle,
  RotateCcw,
} from "lucide-react";
import type { Error } from "@/shared/api/errors";
import { cn } from "@/shared/lib/utils";
import { Button } from "@/shared/components/ui/button";
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
} from "@/shared/components/ui/card";

interface ErrorCardProps {
  errors: Error[];
  refetch: () => void;
}

const errorTypeIcon = {
  validation: AlertTriangle,
  not_found: FileX,
  failure: ServerCrash,
  conflict: Ban,
} as const;

const errorTypeColor = {
  validation: "text-amber-600 dark:text-amber-400",
  not_found: "text-orange-600 dark:text-orange-400",
  failure: "text-destructive",
  conflict: "text-red-600 dark:text-red-400",
} as const;

const errorTypeLabel = {
  validation: "Validation Error",
  not_found: "Not Found",
  failure: "Server Error",
  conflict: "Conflict",
} as const;

function ErrorCard({ errors, refetch }: ErrorCardProps) {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [expanded, setExpanded] = useState(true);

  if (!errors?.length) return null;

  const error = errors[currentIndex];
  const type = error.type.toLowerCase() as keyof typeof errorTypeColor;
  const Icon = errorTypeIcon[type] ?? HelpCircle;
  const total = errors.length;
  const hasMultiple = total > 1;

  return (
    <Card className="border-destructive/30">
      <CardHeader className="my-[-7]">
        <div className="flex items-start justify-between gap-3">
          <div className="flex items-start gap-3 ">
            <Icon
              className={cn("mt-0.5 size-5 shrink-0", errorTypeColor[type])}
            />
            <div>
              <CardTitle className="text-base">
                {errorTypeLabel[type]}
              </CardTitle>
            </div>
            <Button variant="secondary" className="h-6" onClick={refetch}>
              Retry
              <RotateCcw className="h-4 w-4" />
            </Button>
          </div>
          <div className="flex items-center gap-1 shrink-0">
            {hasMultiple && (
              <>
                <Button
                  variant="ghost"
                  size="icon-xs"
                  disabled={currentIndex === 0}
                  onClick={() => setCurrentIndex((i) => i - 1)}
                >
                  <ChevronLeft />
                </Button>
                <span className="text-xs text-muted-foreground tabular-nums min-w-8 text-center">
                  {currentIndex + 1}/{total}
                </span>
                <Button
                  variant="ghost"
                  size="icon-xs"
                  disabled={currentIndex === total - 1}
                  onClick={() => setCurrentIndex((i) => i + 1)}
                >
                  <ChevronRight />
                </Button>
              </>
            )}
            <Button
              variant="ghost"
              size="icon-xs"
              onClick={() => setExpanded((e) => !e)}
            >
              {expanded ? <ChevronUp /> : <ChevronDown />}
            </Button>
          </div>
        </div>
      </CardHeader>
      {expanded && (
        <CardContent className="pt-0">
          <div className="flex flex-col gap-1.5 rounded-lg bg-muted p-3 text-sm">
            <div className="flex items-baseline gap-2">
              <span className="text-muted-foreground text-xs font-medium w-24 shrink-0">
                Code
              </span>
              <code className="rounded bg-muted-foreground/10 px-1.5 py-0.5 font-mono text-xs">
                {error.code}
              </code>
            </div>
            <div className="flex items-baseline gap-2">
              <span className="text-muted-foreground text-xs font-medium w-24 shrink-0">
                Message
              </span>
              <span className="text-foreground">{error.message}</span>
            </div>
            <div className="flex items-baseline gap-2">
              <span className="text-muted-foreground text-xs font-medium w-24 shrink-0">
                Type
              </span>
              <span className={cn("text-xs font-medium", errorTypeColor[type])}>
                {type}
              </span>
            </div>
            {error.invalidField && (
              <div className="flex items-baseline gap-2">
                <span className="text-muted-foreground text-xs font-medium w-24 shrink-0">
                  Invalid Field
                </span>
                <code className="rounded bg-muted-foreground/10 px-1.5 py-0.5 font-mono text-xs">
                  {error.invalidField}
                </code>
              </div>
            )}
          </div>
        </CardContent>
      )}
    </Card>
  );
}

export { ErrorCard };

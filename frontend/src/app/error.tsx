"use client";

import { AlertTriangle } from "lucide-react";
import { ErrorLabel } from "@/shared/components/errors/error-label";
import { Button } from "@/shared/components/ui/button";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/shared/components/ui/card";

interface ErrorProps {
  error: Error & { digest?: string };
  reset: () => void;
}

export default function Error({ error, reset }: ErrorProps) {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center p-8">
      <Card className="w-full max-w-md border-destructive/30">
        <CardHeader className="items-center text-center">
          <AlertTriangle className="size-8 text-destructive" />
          <CardTitle>Something went wrong</CardTitle>
          <CardDescription>
            An unexpected error occurred while loading this page.
          </CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col items-center gap-3">
          <ErrorLabel>{error.message}</ErrorLabel>
          {error.digest && (
            <span className="text-xs text-muted-foreground">
              Error digest: {error.digest}
            </span>
          )}
        </CardContent>
        <CardFooter className="justify-center">
          <Button variant="destructive" onClick={reset}>
            Try again
          </Button>
        </CardFooter>
      </Card>
    </div>
  );
}

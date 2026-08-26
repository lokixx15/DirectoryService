import { LoaderCircle } from "lucide-react";
import { cn } from "@/shared/lib/utils";
import { Button } from "../ui/button";
import { ReactNode } from "react";

const styles = `
@keyframes load-more-pulse {
  0%, 100% {
    opacity: 1;
    filter: brightness(1);
  }
  50% {
    opacity: 0.35;
    filter: brightness(0.7);
  }
}
.load-more-pulsing {
  animation: load-more-pulse 1.2s ease-in-out infinite;
}
`;

interface LoadMoreButtonProps {
  totalElements?: number;
  hasMore?: boolean;
  pageSize: number;
  onPageSizeChange: (pageSize: number) => void;
  onPointerEnter?: () => void;
  loading?: boolean;
  className?: string;
  size?: "xxs" | "xs" | "sm" | "default" | "lg";
  children?: ReactNode;
}

export function LoadMoreButton({
  totalElements,
  hasMore,
  pageSize,
  onPageSizeChange,
  onPointerEnter,
  className,
  loading,
  size = "default",
  children,
}: LoadMoreButtonProps) {
  const handleClick = () => {
    if (!loading) {
      const nextPageSize = pageSize + 5;
      const newPageSize =
        totalElements !== undefined
          ? Math.min(nextPageSize, totalElements)
          : nextPageSize;
      onPageSizeChange(newPageSize);
    }
  };

  const shouldRender = loading
    ? true
    : totalElements !== undefined
      ? pageSize < totalElements
      : (hasMore ?? true);

  if (!shouldRender) {
    return null;
  }

  return (
    <>
      {loading && <style>{styles}</style>}
      <Button
        variant="secondary"
        size={size}
        onPointerEnter={onPointerEnter}
        onClick={handleClick}
        disabled={loading}
        className={cn(className, loading && "load-more-pulsing")}
      >
        {loading && <LoaderCircle className="h-4 w-4 animate-spin mr-2" />}
        {loading ? "Loading..." : children ? children : "Load more"}
      </Button>
    </>
  );
}

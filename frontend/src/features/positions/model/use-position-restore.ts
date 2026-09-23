import { positionsQueryOptions } from "@/entities/positions/api";
import { useMutation } from "@tanstack/react-query";

export function usePositionRestore() {
  const mutation = useMutation(positionsQueryOptions.restoreLocationOptions());

  return {
    restorePosition: mutation.mutate,
    isPending: mutation.isPending,
  };
}

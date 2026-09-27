import { locationsQueryOptions } from "@/entities/locations";
import { useMutation } from "@tanstack/react-query";

export function useRestoreLocation() {
  const mutation = useMutation(locationsQueryOptions.restoreLocationOptions());

  return {
    restoreLocation: mutation.mutate,
    isPending: mutation.isPending,
  };
}

import { locationsQueryOptions } from "@/entities/locations";
import { useMutation } from "@tanstack/react-query";

export function useCreateLocation() {
  const mutation = useMutation(locationsQueryOptions.createLocationOptions());

  return {
    createLocation: mutation.mutate,
    isPending: mutation.isPending,
  };
}

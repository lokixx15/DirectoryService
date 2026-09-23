import { departmentsQueryOptions } from "@/entities/departments";
import { useMutation } from "@tanstack/react-query";

export function useDepartmentRestore() {
  const mutation = useMutation(
    departmentsQueryOptions.restoreDepartmentOptions(),
  );

  return {
    restoreDepartment: mutation.mutate,
    isPending: mutation.isPending,
  };
}

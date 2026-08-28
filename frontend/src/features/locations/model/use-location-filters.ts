import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { useCallback, useRef } from "react";

export function useLocationFilters() {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  const addedDepartmentIds = searchParams.getAll("selectedDepartmentIds");
  const excludedDepartmentIds = searchParams.getAll("excludedDepartmentIds");

  const pendingParamsRef = useRef<URLSearchParams | null>(null);
  const microtaskScheduledRef = useRef(false);

  const updateQueryParams = useCallback(
    (updater: (params: URLSearchParams) => void) => {
      if (!pendingParamsRef.current) {
        const currentSearch =
          typeof window !== "undefined"
            ? window.location.search
            : searchParams.toString();

        pendingParamsRef.current = new URLSearchParams(currentSearch);
      }

      updater(pendingParamsRef.current);
      pendingParamsRef.current.set("page", "1");

      if (!microtaskScheduledRef.current) {
        microtaskScheduledRef.current = true;

        queueMicrotask(() => {
          if (pendingParamsRef.current) {
            router.replace(
              `${pathname}?${pendingParamsRef.current.toString()}`,
              {
                scroll: false,
              },
            );

            pendingParamsRef.current = null;
          }

          microtaskScheduledRef.current = false;
        });
      }
    },
    [pathname, router, searchParams],
  );

  const setAddedDepartmentIdsHandler = useCallback(
    (ids: string[]) => {
      updateQueryParams((params) => {
        params.delete("selectedDepartmentIds");
        ids.forEach((id) => params.append("selectedDepartmentIds", id));
      });
    },
    [updateQueryParams],
  );

  const setExcludedDepartmentIdsHandler = useCallback(
    (ids: string[]) => {
      updateQueryParams((params) => {
        params.delete("excludedDepartmentIds");
        ids.forEach((id) => params.append("excludedDepartmentIds", id));
      });
    },
    [updateQueryParams],
  );

  return {
    addedDepartmentIds,
    excludedDepartmentIds,
    setAddedDepartmentIdsHandler,
    setExcludedDepartmentIdsHandler,
  };
}

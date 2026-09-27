import { usePathname, useRouter, useSearchParams } from "next/navigation";

export function useDepartmentFilters() {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  const rawIsActive = searchParams.get("isActive");
  const isActive =
    rawIsActive === "true" ? true : rawIsActive === "false" ? false : true;

  const setIsActive = (isActive: boolean | undefined) => {
    const params = new URLSearchParams(searchParams?.toString());

    if (isActive !== undefined) {
      params.set("isActive", String(isActive));
    } else {
      params.delete("isActive");
    }

    router.replace(`${pathname}?${params.toString()}`, { scroll: false });
  };

  return {
    isActive,
    setIsActive,
  };
}

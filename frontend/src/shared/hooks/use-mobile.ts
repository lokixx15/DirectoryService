import * as React from "react";

const MOBILE_BREAKPOINT = 768;

const mediaQuery = `(max-width: ${MOBILE_BREAKPOINT - 1}px)`;

function getIsMobile() {
  if (typeof window === "undefined") return false;
  return window.innerWidth < MOBILE_BREAKPOINT;
}

export function useIsMobile() {
  return React.useSyncExternalStore(
    (onStoreChange) => {
      const mql = window.matchMedia(mediaQuery);
      const onChange = () => onStoreChange();
      mql.addEventListener("change", onChange);
      return () => mql.removeEventListener("change", onChange);
    },
    getIsMobile,
    () => false,
  );
}

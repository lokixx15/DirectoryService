import { LocationTableWidget } from "@/widgets/locations";
import { Suspense } from "react";

export default function Home() {
  return (
    <Suspense>
      <LocationTableWidget />
    </Suspense>
  );
}

import { LocationTableWidget } from "@/widgets/locations/ui/location-table-widget";
import { Suspense } from "react";

export default function Home() {
  return (
    <Suspense>
      <LocationTableWidget />
    </Suspense>
  );
}

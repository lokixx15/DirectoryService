import { PositionListWidget } from "@/widgets/positions/ui/position-list-widget";
import { Suspense } from "react";

export default function Home() {
  return (
    <Suspense>
      <PositionListWidget />
    </Suspense>
  );
}

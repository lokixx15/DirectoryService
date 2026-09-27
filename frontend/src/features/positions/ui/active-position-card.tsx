import { Position } from "@/entities/positions/types";
import { ActivitySwitcher } from "@/shared/components/switches/activity-switcher";
import { Badge } from "@/shared/components/ui/badge";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card";
import { FormatDate } from "@/shared/lib/format-date";

interface ActivePositionCardProps {
  position: Position;
  handleActiveSwitch: (value: boolean) => void;
}

export function ActivePositionCard({
  position,
  handleActiveSwitch,
}: ActivePositionCardProps) {
  return (
    <Card
      key={position.id}
      className="transition-all duration-200 hover:shadow-md hover:-translate-y-0.5 border-l-4 border-l-primary"
    >
      <CardHeader className="pb-3">
        <div className="flex items-start justify-between gap-2">
          <CardTitle>{position.name}</CardTitle>
          <Badge variant="success">Active</Badge>
        </div>
        {position.description && (
          <p className="text-sm text-muted-foreground line-clamp-2">
            {position.description}
          </p>
        )}
      </CardHeader>
      <CardContent>
        <div className="flex flex-col gap-x-4 gap-y-1.5 text-xs">
          <span>Created: {FormatDate(position.createdAt)}</span>
          <span>Updated: {FormatDate(position.updatedAt)}</span>
          <ActivitySwitcher
            isActive={position.isActive}
            onIsActiveChange={handleActiveSwitch}
          />
        </div>
      </CardContent>
    </Card>
  );
}

import { Switch } from "../ui/switch";

interface ActivitySwitcherProps {
  isActive: boolean;
  onIsActiveChange: (isActive: boolean) => void;
}

export function ActivitySwitcher({
  isActive,
  onIsActiveChange,
}: ActivitySwitcherProps) {
  return (
    <Switch
      checked={isActive}
      onCheckedChange={onIsActiveChange}
      className="h-4 w-7 [&_span]:h-3 [&_span]:w-3 data-[state=checked]:[&_span]:translate-x-3 data-[state=checked]:bg-emerald-500 data-[state=unchecked]:bg-red-500"
      onClick={(e) => e.stopPropagation()}
    />
  );
}

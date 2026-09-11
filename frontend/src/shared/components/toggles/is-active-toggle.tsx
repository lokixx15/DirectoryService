import { Toggle } from "../ui/toggle";

interface IsActiveToggleProps {
  isActive?: boolean;
  onIsActiveChange: (isActive: boolean) => void;
}

export function IsActiveToggle({
  isActive = true,
  onIsActiveChange,
}: IsActiveToggleProps) {
  return (
    <Toggle size="sm" variant="outline" pressed={isActive} onPressedChange={onIsActiveChange}>
      <span>{isActive ? "Active" : "Archive"}</span>
    </Toggle>
  );
}

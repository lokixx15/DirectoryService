import { Toggle } from "../ui/toggle";

interface ActiveToggleProps {
  isActive: boolean;
  onIsActiveChange: (isActive: boolean) => void;
  className?: string;
  activeText?: string;
  inActiveText?: string;
}

export function ActiveToggle({
  isActive,
  onIsActiveChange,
  className,
  activeText = "Active",
  inActiveText = "Inactive",
}: ActiveToggleProps) {
  return (
    <Toggle
      size="sm"
      variant="outline"
      pressed={isActive}
      onPressedChange={onIsActiveChange}
      className={className}
    >
      <span>{isActive ? activeText : inActiveText}</span>
    </Toggle>
  );
}

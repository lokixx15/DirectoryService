export function FormatDate(date: Date): string {
  if (!date) return "unknown date";
  return new Date(date).toLocaleString("en-US", {
    timeZone: "UTC",
  });
}

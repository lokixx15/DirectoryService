export function FormatDate(date: Date): string {
  return new Date(date).toLocaleString("en-US", {
    timeZone: "UTC",
  });
}

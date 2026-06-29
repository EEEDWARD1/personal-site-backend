export function splitCommaList(value?: string) {
  return (
    value
      ?.split(",")
      .map((item) => item.trim())
      .filter(Boolean) ?? []
  );
}

export function splitParagraphs(value?: string, fallback = "") {
  return (value || fallback).split(/\n{2,}/).filter(Boolean);
}

export function formatDisplayDate(value?: string) {
  if (!value) return null;

  return new Intl.DateTimeFormat("en-GB", {
    day: "numeric",
    month: "long",
    year: "numeric",
  }).format(new Date(value));
}

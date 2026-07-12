import events from "../data/events.json";
import facets from "../data/facets.json";

export type EventItem = {
  id: number;
  date: string;
  chain: string;
  venue: string;
  prefecture: string;
  pros: string[];
  proText: string;
  timeSlots: string[];
  sourceUrl: string;
  scrapedAt: string;
};

export const allEvents = (events as EventItem[]).sort((a, b) =>
  a.date.localeCompare(b.date) || a.venue.localeCompare(b.venue, "ja")
);

export const siteFacets = facets as {
  generatedAt: string;
  eventCount: number;
  chains: string[];
  prefectures: string[];
  venues: string[];
  pros: string[];
};

export function byPrefecture(name: string) {
  return allEvents.filter((event) => event.prefecture === name);
}

export function byVenue(name: string) {
  return allEvents.filter((event) => event.venue === name);
}

export function byChain(name: string) {
  return allEvents.filter((event) => event.chain === name);
}

export function byPro(name: string) {
  return allEvents.filter((event) => event.pros.includes(name) || event.proText.includes(name));
}

export function unique(values: string[]) {
  return [...new Set(values.filter(Boolean))].sort((a, b) => a.localeCompare(b, "ja"));
}

export function pageSlug(value: string) {
  return encodeURIComponent(value);
}

export function fromPageSlug(value: string) {
  return decodeURIComponent(value);
}

export function eventSlug(event: EventItem) {
  return `${event.id}-${event.date}-${event.venue}`;
}

export function findEventBySlug(slug: string) {
  const id = Number(slug.split("-", 1)[0]);
  return allEvents.find((event) => event.id === id);
}

export function formatDate(value: string) {
  return new Intl.DateTimeFormat("ja-JP", {
    year: "numeric",
    month: "long",
    day: "numeric",
    weekday: "short"
  }).format(new Date(`${value}T00:00:00+09:00`));
}

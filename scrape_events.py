import datetime
import json
import re
from typing import List, Optional
from urllib.parse import urljoin

import requests
from bs4 import BeautifulSoup

BASE_URL = "https://www.rokkobowl.co.jp"
EVENT_LIST_URL = f"{BASE_URL}/archives/category/event"


def _infer_date(month: int, day: int, today: datetime.date) -> datetime.date:
    candidate = datetime.date(today.year, month, day)
    if candidate < today:
        candidate = datetime.date(today.year + 1, month, day)
    return candidate


def _extract_dates(soup: BeautifulSoup, today: datetime.date) -> List[datetime.date]:
    dates: List[datetime.date] = []

    for el in soup.find_all("span"):
        text = el.get_text(strip=True)
        match = re.fullmatch(r"(\d{1,2})/(\d{1,2})", text)
        if match:
            month, day = map(int, match.groups())
            dates.append(_infer_date(month, day, today))

    page_text = soup.get_text(" ", strip=True)
    for match in re.finditer(r"(\d{1,2})月(\d{1,2})日", page_text):
        month, day = map(int, match.groups())
        dates.append(_infer_date(month, day, today))

    return sorted(set(dates))


def _pick_event_date(dates: List[datetime.date], today: datetime.date) -> datetime.date:
    future_dates = [d for d in dates if d >= today]
    if future_dates:
        return min(future_dates)
    if dates:
        return min(dates)
    return today + datetime.timedelta(days=30)


def _fetch_detail(url: str) -> BeautifulSoup:
    response = requests.get(url, timeout=30)
    response.raise_for_status()
    return BeautifulSoup(response.text, "html.parser")


def scrape_kobe_rokkou() -> List[dict]:
    today = datetime.date.today()
    results: List[dict] = []
    seen_urls = set()

    response = requests.get(EVENT_LIST_URL, timeout=30)
    response.raise_for_status()
    soup = BeautifulSoup(response.text, "html.parser")

    for heading in soup.select("h3.p_eventTit"):
        link = heading.find("a", href=True)
        if not link:
            continue

        title = link.get_text(strip=True)
        if "プロ" not in title:
            continue

        detail_url = urljoin(BASE_URL, link["href"])
        if detail_url in seen_urls:
            continue
        seen_urls.add(detail_url)

        detail_soup = _fetch_detail(detail_url)
        dates = _extract_dates(detail_soup, today)
        event_date = _pick_event_date(dates, today)

        results.append(
            {
                "title": title,
                "date": event_date.isoformat(),
                "area": "Hyogo",
                "ticket_aff": False,
                "ticket_url": None,
                "detail_url": detail_url,
            }
        )

    return results


if __name__ == "__main__":
    data = scrape_kobe_rokkou()
    print(json.dumps(data, ensure_ascii=False, indent=2))
    with open("events.json", "w", encoding="utf-8") as file:
        json.dump(data, file, ensure_ascii=False, indent=2)

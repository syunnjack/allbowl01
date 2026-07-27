import datetime
import json
from pathlib import Path
from typing import List, Literal, Optional

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field

app = FastAPI(title="Bowling Event API")

app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "https://bowling-event.jp",
        "http://127.0.0.1:4321",
        "http://localhost:4321",
    ],
    allow_methods=["GET", "POST"],
    allow_headers=["*"],
)

DATA_DIR = Path(__file__).with_name("data")
UGC_TIPS_PATH = DATA_DIR / "ugc_tips.json"

# ---------- データモデル ----------


class Fan(BaseModel):
    area: str
    favorite_pro_ids: List[int]
    budget: int


class Event(BaseModel):
    id: int
    title: str
    pro_id: int
    area: str
    date: datetime.date
    ticket_aff: bool
    ticket_url: Optional[str] = None


class Item(BaseModel):
    id: int
    name: str
    pro_id: int
    price: int
    url: str


class Offer(BaseModel):
    event: Event
    bundle: List[Item]
    total: int
    score: float


class UgcTipCreate(BaseModel):
    kind: Literal["visit", "correction", "tip"] = "tip"
    eventId: Optional[int] = None
    venue: str = Field(min_length=1, max_length=120)
    nickname: str = Field(min_length=1, max_length=30)
    body: str = Field(min_length=1, max_length=1000)


class UgcTip(UgcTipCreate):
    id: int
    status: Literal["pending", "approved", "rejected"] = "pending"
    createdAt: str


# ---------- UGC storage ----------


def load_ugc_tips() -> List[UgcTip]:
    if not UGC_TIPS_PATH.exists():
        return []
    raw = json.loads(UGC_TIPS_PATH.read_text(encoding="utf-8"))
    return [UgcTip(**entry) for entry in raw]


def save_ugc_tips(tips: List[UgcTip]) -> None:
    DATA_DIR.mkdir(parents=True, exist_ok=True)
    UGC_TIPS_PATH.write_text(
        json.dumps([tip.model_dump() for tip in tips], ensure_ascii=False, indent=2),
        encoding="utf-8",
    )


# ---------- データ読み込み ----------

items_db = [
    Item(id=1, name="プロモデルバレル", pro_id=101, price=12000, url="https://aff.example.com/barrel"),
    Item(id=2, name="フライトセット", pro_id=101, price=2000, url="https://aff.example.com/flight"),
    Item(id=3, name="応援タオル", pro_id=101, price=3000, url="https://aff.example.com/towel"),
]


def load_events_db() -> List[Event]:
    events_path = Path(__file__).with_name("events.json")
    if not events_path.exists():
        return []

    raw = json.loads(events_path.read_text(encoding="utf-8"))
    return [
        Event(
            id=index,
            title=entry["title"],
            pro_id=101,
            area=entry["area"],
            date=datetime.date.fromisoformat(entry["date"]),
            ticket_aff=entry["ticket_aff"],
            ticket_url=entry.get("ticket_url"),
        )
        for index, entry in enumerate(raw, start=1)
    ]


events_db = load_events_db()


# ---------- ロジック ----------


def calc_score(fan: Fan, ev: Event, items: List[Item]) -> float:
    score = 0.0

    if ev.pro_id in fan.favorite_pro_ids:
        score += 50.0

    days = (ev.date - datetime.date.today()).days
    if 0 <= days <= 14:
        score += float(20 - days)

    total = sum(item.price for item in items)
    if fan.budget > 0:
        ratio = total / fan.budget
        if 0.7 <= ratio <= 1.1:
            score += 30.0
        elif 0.4 <= ratio < 0.7:
            score += 10.0

    if ev.ticket_aff:
        score += 15.0

    return score


def generate_offers(fan: Fan) -> List[Offer]:
    offers: List[Offer] = []

    for ev in events_db:
        if ev.area != fan.area:
            continue

        if ev.pro_id not in fan.favorite_pro_ids:
            continue

        pro_items = [item for item in items_db if item.pro_id == ev.pro_id]
        pro_items.sort(key=lambda item: item.price, reverse=True)

        bundle: List[Item] = []
        total = 0

        for item in pro_items:
            if total + item.price <= int(fan.budget * 1.1):
                bundle.append(item)
                total += item.price

        if not bundle:
            continue

        score = calc_score(fan, ev, bundle)

        offers.append(
            Offer(
                event=ev,
                bundle=bundle,
                total=total,
                score=score,
            )
        )

    offers.sort(key=lambda offer: offer.score, reverse=True)
    return offers


# ---------- エンドポイント ----------


@app.post("/offers", response_model=List[Offer])
def get_offers(fan: Fan):
    return generate_offers(fan)


@app.get("/ugc/tips", response_model=List[UgcTip])
def list_ugc_tips(status: Optional[str] = "approved", event_id: Optional[int] = None):
    tips = load_ugc_tips()
    if status:
        tips = [tip for tip in tips if tip.status == status]
    if event_id is not None:
        tips = [tip for tip in tips if tip.eventId == event_id]
    return tips


@app.post("/ugc/tips", response_model=UgcTip)
def create_ugc_tip(payload: UgcTipCreate):
    tips = load_ugc_tips()
    next_id = max((tip.id for tip in tips), default=0) + 1
    tip = UgcTip(
        id=next_id,
        status="pending",
        createdAt=datetime.date.today().isoformat(),
        **payload.model_dump(),
    )
    tips.append(tip)
    save_ugc_tips(tips)
    return tip


if __name__ == "__main__":
    import uvicorn

    uvicorn.run("main:app", host="127.0.0.1", port=8000, reload=True)

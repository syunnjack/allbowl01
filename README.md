# dart-portal

ダーツプロ総合アフィリエイトサイトの MVP バックエンド。  
イベント情報の自動収集と、推しプロ × イベント × ギアのレコメンド API を提供します。

## セットアップ

```powershell
python -m venv .venv
.\.venv\Scripts\pip.exe install -r requirements.txt
```

## イベント情報の更新

```powershell
.\.venv\Scripts\python.exe scrape_events.py
```

神戸六甲ボウルの来店イベントを取得し、`events.json` に保存します。

## API 起動

```powershell
.\.venv\Scripts\python.exe -m uvicorn main:app --host 127.0.0.1 --port 8765
```

- Swagger UI: http://127.0.0.1:8765/docs
- レコメンド API: `POST /offers`

### リクエスト例

```json
{
  "area": "Hyogo",
  "favorite_pro_ids": [101],
  "budget": 20000
}
```

## 設計書

全体設計・API・CMS・ロードマップは [docs/DESIGN.md](docs/DESIGN.md) を参照してください。

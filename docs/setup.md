# Setup

## Server

```bash
cd server
npm install
npm run dev
```

Or use Docker Compose from the repo root:

```bash
docker compose up --build
```

## Unity Client

1. Open `client-unity` in Unity 2022 LTS.
2. Install TextMeshPro essentials and the Colyseus Unity SDK.
3. Configure `Assets/Config/AppConfig.json` for server endpoint and GPS settings.

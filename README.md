# worldrift-prototype

Monorepo skeleton for a GPS-required, mobile, pixel-art 2D multiplayer action RPG prototype inspired by Realm of the Mad God.

## Quickstart (Server)

```bash
docker compose up --build
```

The Colyseus server runs at `ws://localhost:2567` and exposes `http://localhost:2567/health`.

## Unity Client Setup

1. Install Unity 2022 LTS with Android/iOS build support.
2. Open `client-unity` as a Unity project.
3. Install TextMeshPro essentials when prompted (needed for UI).
4. Add the Colyseus Unity SDK via Package Manager (see `client-unity/Packages/manifest.json`).
5. Open the Boot scene and press Play.

## Simulated GPS Mode

Simulated GPS is disabled by default. Enable it by editing `client-unity/Assets/Config/AppConfig.json`:

```json
{
  "simulatedGpsEnabled": true,
  "simulatedGpsLat": 37.7749,
  "simulatedGpsLon": -122.4194
}
```

Use this only for local development.

## Repository Layout

- `client-unity/`: Unity client project (2D, mobile)
- `server/`: Node.js + TypeScript Colyseus server
- `shared/`: JSON schemas and shared types/constants
- `docs/`: Architecture and setup notes

## License

MIT

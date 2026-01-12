# Architecture

## Client Responsibilities

- GPS permission gate and location tracking
- POI discovery and distance checks (client-side UX)
- Connects to Colyseus server and sends player input
- Renders player sprites and other players in the room

## Server Responsibilities

- Authoritative movement simulation
- Player state sync and room lifecycle
- POI entry validation based on provided GPS coordinates
- Health endpoint for monitoring

## Authoritative Movement

The server receives `input` messages, applies a fixed speed with delta time, and syncs updated positions.

## POI Gating

Clients send POI metadata and their GPS coordinates when joining. The server validates distance using the Haversine formula and rejects players outside the enter radius.

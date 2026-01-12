import { Room, Client } from 'colyseus';
import { Schema, type, MapSchema } from '@colyseus/schema';
import { haversineMeters } from '../util/geo';
import {
  MESSAGE_INPUT,
  ENTER_RADIUS_METERS,
  MESSAGE_CHAT
} from '../shared/messages';
import fs from 'fs';
import path from 'path';

class PlayerState extends Schema {
  @type('string') id = '';
  @type('string') name = '';
  @type('string') faction = '';
  @type('number') x = 0;
  @type('number') y = 0;
  @type('number') lat = 0;
  @type('number') lon = 0;
  @type('number') lastSeen = 0;
}

class RiftState extends Schema {
  @type({ map: PlayerState }) players = new MapSchema<PlayerState>();
}

type InputPayload = {
  x: number;
  y: number;
};

type POI = {
  id: string;
  name: string;
  lat: number;
  lon: number;
  seed: number;
};

export class RiftRoom extends Room<RiftState> {
  private inputMap = new Map<string, InputPayload>();
  private pois: POI[] = [];

  onCreate(): void {
    this.setState(new RiftState());
    this.setSimulationInterval((deltaTime) => this.update(deltaTime));
    this.pois = loadSamplePois();

    this.onMessage(MESSAGE_INPUT, (client, message: InputPayload) => {
      this.inputMap.set(client.sessionId, message);
    });

    this.onMessage(MESSAGE_CHAT, (client, message: string) => {
      this.broadcast(MESSAGE_CHAT, `${client.sessionId}: ${message}`);
    });
  }

  onJoin(client: Client, options: any): void {
    const player = new PlayerState();
    player.id = client.sessionId;
    player.name = options.name ?? 'Rift-Player';
    player.faction = options.faction ?? 'Meridian Covenant';
    player.lat = Number(options.lat ?? 0);
    player.lon = Number(options.lon ?? 0);
    player.lastSeen = Date.now();

    if (options.poiId) {
      const poi = this.pois.find((item) => item.id === options.poiId);
      if (poi) {
        const distance = haversineMeters(player.lat, player.lon, poi.lat, poi.lon);
        if (distance > ENTER_RADIUS_METERS) {
          throw new Error('Too far from POI');
        }
      }
    }

    // TODO: Add anti-cheat validation for GPS data (server-side verification).
    this.state.players.set(client.sessionId, player);
  }

  onLeave(client: Client): void {
    this.state.players.delete(client.sessionId);
    this.inputMap.delete(client.sessionId);
  }

  private update(deltaTime: number): void {
    const dt = deltaTime / 1000;
    const speed = 3;

    this.state.players.forEach((player, id) => {
      const input = this.inputMap.get(id);
      if (!input) {
        return;
      }

      player.x += input.x * speed * dt;
      player.y += input.y * speed * dt;
      player.lastSeen = Date.now();
    });
  }
}

function loadSamplePois(): POI[] {
  const filePath = path.resolve(__dirname, '..', '..', '..', 'shared', 'sample_pois.json');
  if (!fs.existsSync(filePath)) {
    return [];
  }

  const json = fs.readFileSync(filePath, 'utf-8');
  const parsed = JSON.parse(json) as { pois?: POI[] };
  return parsed.pois ?? [];
}

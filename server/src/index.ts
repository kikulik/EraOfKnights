import { createServer } from 'http';
import express from 'express';
import { Server } from 'colyseus';
import { RiftRoom } from './rooms/RiftRoom';
import { config } from './util/config';

const app = express();
app.get('/health', (_req, res) => {
  res.json({ status: 'ok' });
});

const server = createServer(app);
const gameServer = new Server({
  server
});

gameServer.define('rift', RiftRoom);

gameServer.listen(config.port);
console.log(`Worldrift server listening on ws://localhost:${config.port}`);

export const MESSAGE_INPUT = 'input';
export const MESSAGE_ENTER_POI = 'enter_poi';
export const MESSAGE_CHAT = 'chat';

export const FACTIONS = {
  MERIDIAN: 'Meridian Covenant',
  UMBRAL: 'Umbral Relay'
} as const;

export type Faction = (typeof FACTIONS)[keyof typeof FACTIONS];

export const ENTER_RADIUS_METERS = 150;

export type InputMessage = {
  x: number;
  y: number;
};

export type EnterPoiMessage = {
  poiId: string;
  lat: number;
  lon: number;
};

export type ChatMessage = string;

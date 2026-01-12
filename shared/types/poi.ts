export type POI = {
  id: string;
  name: string;
  lat: number;
  lon: number;
  seed: number;
};

export type POIList = {
  pois: POI[];
};

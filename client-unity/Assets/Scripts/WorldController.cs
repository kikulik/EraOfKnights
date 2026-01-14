using UnityEngine;

namespace Worldrift.Client
{
    public class WorldController : MonoBehaviour
    {
        [SerializeField] private WorldUIController uiController;
        [SerializeField] private GPSManager gpsManager;
        [SerializeField] private WorldPOIService poiService;
        [SerializeField] private SceneFlowController sceneFlow;
        [SerializeField] private GeoLocalizer geoLocalizer;
        [SerializeField] private WorldChunkRenderer chunkRenderer;
        [SerializeField] private PlayerAvatar playerAvatar;
        [SerializeField] private PortalMarker portalMarkerPrefab;
        [SerializeField] private Transform portalParent;
        [SerializeField] private int maxPortalMarkers = 5;
        [SerializeField] private Color portalColor = new Color(0.7f, 0.2f, 0.9f, 1f);

        private AppConfig config;
        private GpsLocation lastLocation;
        private bool hasFirstLocation;
        private readonly System.Collections.Generic.List<PortalMarker> portalMarkers = new System.Collections.Generic.List<PortalMarker>();

        private void Awake()
        {
            config = AppConfig.Load();

            // Safety logs (helps wiring/debug)
            Debug.Log("[WORLD] WorldController Awake");
            Debug.Log($"[WORLD] uiController ref: {(uiController == null ? "NULL" : "OK")}");
            Debug.Log($"[WORLD] gpsManager ref: {(gpsManager == null ? "NULL" : "OK")}");
            Debug.Log($"[WORLD] poiService ref: {(poiService == null ? "NULL" : "OK")}");
            Debug.Log($"[WORLD] sceneFlow ref: {(sceneFlow == null ? "NULL" : "OK")}");

            if (poiService != null)
            {
                poiService.LoadPOIs();
            }

            if (geoLocalizer == null)
            {
                geoLocalizer = FindObjectOfType<GeoLocalizer>();
            }

            if (chunkRenderer == null)
            {
                chunkRenderer = FindObjectOfType<WorldChunkRenderer>();
            }

            if (playerAvatar == null)
            {
                playerAvatar = FindObjectOfType<PlayerAvatar>();
            }

            if (portalParent == null)
            {
                var parentObject = new GameObject("PortalMarkers");
                parentObject.transform.SetParent(transform, false);
                portalParent = parentObject.transform;
            }

            if (gpsManager != null)
            {
                gpsManager.Initialize(config);
                gpsManager.StatusChanged += OnGpsStatusChanged;
                gpsManager.LocationUpdated += OnLocationUpdated;
            }

            if (uiController != null)
            {
                uiController.PoiSelected += OnPoiSelected;
                uiController.SetStatus("WORLD LOADED");
            }
        }

        private void Start()
        {
            Debug.Log("[WORLD] WorldController Start");

            if (gpsManager != null)
            {
                gpsManager.StartLocation();
            }

            if (uiController != null)
            {
                uiController.SetStatus("Waiting for GPS...");
            }
        }

        private void OnDestroy()
        {
            // Clean up subscriptions (prevents duplicate calls if scene reloads)
            if (gpsManager != null)
            {
                gpsManager.StatusChanged -= OnGpsStatusChanged;
                gpsManager.LocationUpdated -= OnLocationUpdated;
            }

            if (uiController != null)
            {
                uiController.PoiSelected -= OnPoiSelected;
            }
        }

        private void OnGpsStatusChanged(string status)
        {
            Debug.Log("[WORLD] GPS status: " + status);

            // Optional: show status in UI too
            if (uiController != null)
            {
                uiController.SetStatus(status);
            }
        }

        private void OnLocationUpdated(GpsLocation location)
        {
            lastLocation = location;
            hasFirstLocation = true;

            Debug.Log($"[WORLD] GPS loc: {location.Latitude},{location.Longitude}");

            if (poiService == null || uiController == null) return;

            var nearest = poiService.GetNearest(location.Latitude, location.Longitude);
            uiController.SetPois(nearest, location.Latitude, location.Longitude);

            // Also show a quick “nearest” line (so you can verify flow immediately)
            var top1 = poiService.GetNearest(location.Latitude, location.Longitude, 1);
            uiController.SetStatus("Nearest: " + (top1.Count > 0 ? top1[0].name : "none"));

            if (geoLocalizer != null)
            {
                bool originSet = geoLocalizer.TrySetOrigin(location.Latitude, location.Longitude);
                if (originSet && chunkRenderer != null)
                {
                    int seedOffset = top1.Count > 0 ? top1[0].seed : 0;
                    chunkRenderer.Initialize(location.Latitude, location.Longitude, seedOffset);
                }

                Vector2 playerWorld = geoLocalizer.LatLonToWorldUnits(location.Latitude, location.Longitude);
                if (playerAvatar != null)
                {
                    playerAvatar.SetWorldPosition(playerWorld);
                }

                if (chunkRenderer != null)
                {
                    chunkRenderer.UpdateChunk(playerWorld);
                }

                UpdatePortalMarkers(nearest);
            }
        }

        private void OnPoiSelected(POI poi)
        {
            if (!hasFirstLocation)
            {
                uiController?.SetStatus("No GPS fix yet.");
                return;
            }

            var distance = WorldPOIService.DistanceMeters(
                lastLocation.Latitude, lastLocation.Longitude,
                poi.lat, poi.lon
            );

            if (distance <= config.poiEnterRadiusMeters)
            {
                PlayerPrefs.SetString("poiId", poi.id);
                PlayerPrefs.SetInt("poiSeed", poi.seed);
                PlayerPrefs.SetFloat("poiLat", (float)poi.lat);
                PlayerPrefs.SetFloat("poiLon", (float)poi.lon);

                sceneFlow?.GoToInstance();
            }
            else
            {
                uiController?.SetStatus($"Too far: {distance:0}m away");
            }
        }

        private void UpdatePortalMarkers(System.Collections.Generic.List<POI> pois)
        {
            if (geoLocalizer == null)
            {
                return;
            }

            int count = pois != null ? Mathf.Min(pois.Count, maxPortalMarkers) : 0;
            EnsurePortalMarkers(count);

            for (int i = 0; i < count; i++)
            {
                var poi = pois[i];
                var marker = portalMarkers[i];
                Vector2 worldPosition = geoLocalizer.LatLonToWorldUnits(poi.lat, poi.lon);
                marker.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0f);
                marker.Initialize(poi, poi.name, portalColor);
                marker.gameObject.SetActive(true);
            }

            for (int i = count; i < portalMarkers.Count; i++)
            {
                portalMarkers[i].gameObject.SetActive(false);
            }
        }

        private void EnsurePortalMarkers(int count)
        {
            while (portalMarkers.Count < count)
            {
                PortalMarker marker = null;
                if (portalMarkerPrefab != null)
                {
                    marker = Instantiate(portalMarkerPrefab, portalParent);
                }
                else
                {
                    var markerObject = new GameObject($"PortalMarker_{portalMarkers.Count}");
                    markerObject.transform.SetParent(portalParent, false);
                    markerObject.AddComponent<SpriteRenderer>();
                    marker = markerObject.AddComponent<PortalMarker>();
                }

                if (marker != null)
                {
                    marker.Clicked += OnPoiSelected;
                    portalMarkers.Add(marker);
                }
            }
        }
    }
}

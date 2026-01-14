using UnityEngine;

namespace Worldrift.Client
{
    public class WorldController : MonoBehaviour
    {
        [SerializeField] private WorldUIController uiController;
        [SerializeField] private GPSManager gpsManager;
        [SerializeField] private WorldPOIService poiService;
        [SerializeField] private SceneFlowController sceneFlow;

        private AppConfig config;
        private GpsLocation lastLocation;
        private bool hasFirstLocation;

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
    }
}

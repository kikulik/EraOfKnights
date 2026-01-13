using UnityEngine;

namespace Worldrift.Client
{
    public class BootController : MonoBehaviour
    {
        [SerializeField] private BootUIController uiController;
        [SerializeField] private GPSManager gpsManager;
        [SerializeField] private SceneFlowController sceneFlow;

        private AppConfig config;
        private bool hasLoadedConnect;

        private void Awake()
        {
            config = AppConfig.Load();

            gpsManager.Initialize(config);
            gpsManager.StatusChanged += OnStatusChanged;
            gpsManager.LocationUpdated += OnLocationUpdated;
        }

        private void Start()
        {
            uiController.SetStatus("Checking GPS...");
            gpsManager.StartLocation();
        }

        private void OnStatusChanged(string status)
        {
            if (status.Contains("disabled") || status.Contains("failed") || status.Contains("timeout"))
            {
                uiController.SetBlocking(true, "GPS is required. Please enable location services and restart.");
            }
            else
            {
                uiController.SetBlocking(false, status);
            }
        }

        private void OnLocationUpdated(GpsLocation location)
        {
            // Update UI so we can see GPS values in Editor (simulated GPS).
            uiController.SetStatus($"GPS: {location.Latitude}, {location.Longitude}");

            // Only transition once.
            if (hasLoadedConnect)
            {
                return;
            }

            if (!config.gpsRequired || location.Latitude != 0 || location.Longitude != 0)
            {
                Debug.Log($"[BOOT] Got GPS {location.Latitude},{location.Longitude} -> loading Connect");
                hasLoadedConnect = true;
                // sceneFlow.GoToConnect();
            }
        }
    }
}

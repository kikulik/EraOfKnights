using UnityEngine;

namespace Worldrift.Client
{
    public class BootController : MonoBehaviour
    {
        [SerializeField] private BootUIController uiController;
        [SerializeField] private GPSManager gpsManager;
        [SerializeField] private SceneFlowController sceneFlow;

        private AppConfig config;

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
            if (!config.gpsRequired)
            {
                sceneFlow.GoToConnect();
                return;
            }

            if (location.Latitude != 0 || location.Longitude != 0)
            {
                sceneFlow.GoToConnect();
            }
        }
    }
}

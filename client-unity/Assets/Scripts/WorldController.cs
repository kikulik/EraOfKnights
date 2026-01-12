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

        private void Awake()
        {
            config = AppConfig.Load();
            gpsManager.Initialize(config);
            gpsManager.LocationUpdated += OnLocationUpdated;
            uiController.PoiSelected += OnPoiSelected;
            poiService.LoadPOIs();
        }

        private void Start()
        {
            gpsManager.StartLocation();
        }

        private void OnLocationUpdated(GpsLocation location)
        {
            lastLocation = location;
            var nearest = poiService.GetNearest(location.Latitude, location.Longitude);
            uiController.SetPois(nearest, location.Latitude, location.Longitude);
        }

        private void OnPoiSelected(POI poi)
        {
            var distance = WorldPOIService.DistanceMeters(lastLocation.Latitude, lastLocation.Longitude, poi.lat, poi.lon);
            if (distance <= config.poiEnterRadiusMeters)
            {
                PlayerPrefs.SetString("poiId", poi.id);
                PlayerPrefs.SetInt("poiSeed", poi.seed);
                PlayerPrefs.SetFloat("poiLat", (float)poi.lat);
                PlayerPrefs.SetFloat("poiLon", (float)poi.lon);
                sceneFlow.GoToInstance();
            }
            else
            {
                uiController.SetStatus($"Too far: {distance:0}m away");
            }
        }
    }
}

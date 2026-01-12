using System;
using System.Collections;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Worldrift.Client
{
    public struct GpsLocation
    {
        public double Latitude;
        public double Longitude;
    }

    public class GPSManager : MonoBehaviour
    {
        public event Action<GpsLocation> LocationUpdated;
        public event Action<string> StatusChanged;

        private AppConfig config;
        private bool isRunning;

        public void Initialize(AppConfig appConfig)
        {
            config = appConfig;
        }

        public void StartLocation()
        {
            if (isRunning)
            {
                return;
            }

            if (config != null && config.simulatedGpsEnabled)
            {
                StatusChanged?.Invoke("Simulated GPS active");
                StartCoroutine(SimulatedLocation());
                isRunning = true;
                return;
            }

            StartCoroutine(StartLocationService());
        }

        private IEnumerator SimulatedLocation()
        {
            while (true)
            {
                var simulated = new GpsLocation
                {
                    Latitude = config.simulatedGpsLat,
                    Longitude = config.simulatedGpsLon
                };
                LocationUpdated?.Invoke(simulated);
                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator StartLocationService()
        {
            StatusChanged?.Invoke("Requesting GPS permissions");
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
                yield return new WaitForSeconds(1f);
            }
#endif

            if (!Input.location.isEnabledByUser)
            {
                StatusChanged?.Invoke("GPS disabled");
                yield break;
            }

            Input.location.Start(1f, 0.5f);

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                StatusChanged?.Invoke("Initializing GPS...");
                yield return new WaitForSeconds(1f);
                maxWait--;
            }

            if (maxWait <= 0)
            {
                StatusChanged?.Invoke("GPS init timeout");
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                StatusChanged?.Invoke("GPS failed");
                yield break;
            }

            StatusChanged?.Invoke("GPS active");
            isRunning = true;

            while (true)
            {
                var lastData = Input.location.lastData;
                LocationUpdated?.Invoke(new GpsLocation
                {
                    Latitude = lastData.latitude,
                    Longitude = lastData.longitude
                });
                yield return new WaitForSeconds(1f);
            }
        }
    }
}

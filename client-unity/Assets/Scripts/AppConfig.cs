using System;
using System.IO;
using UnityEngine;

namespace Worldrift.Client
{
    [Serializable]
    public class AppConfig
    {
        public string serverEndpoint = "ws://localhost:2567";
        public bool gpsRequired = true;
        public bool simulatedGpsEnabled = false;
        public double simulatedGpsLat = 37.7749;
        public double simulatedGpsLon = -122.4194;
        public float poiEnterRadiusMeters = 150f;
        public int tickRate = 30;

        public static AppConfig Load()
        {
            var configPath = Path.Combine(Application.dataPath, "Config", "AppConfig.json");
            if (!File.Exists(configPath))
            {
                Debug.LogWarning($"AppConfig.json not found at {configPath}, using defaults.");
                return new AppConfig();
            }

            var json = File.ReadAllText(configPath);
            return JsonUtility.FromJson<AppConfig>(json) ?? new AppConfig();
        }
    }
}

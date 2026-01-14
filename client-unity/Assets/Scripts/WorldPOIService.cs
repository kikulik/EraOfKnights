using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Worldrift.Client
{
    [Serializable]
    public class POI
    {
        public string id;
        public string name;
        public double lat;
        public double lon;
        public int seed;
    }

    [Serializable]
    public class POIList
    {
        public List<POI> pois = new List<POI>();
    }

    public class WorldPOIService : MonoBehaviour
    {
        public POIList LoadedPOIs { get; private set; } = new POIList();

        [Header("Optional: override shared path for debugging")]
        [SerializeField] private string overrideSharedPath = "";

        // Resources path WITHOUT extension:
        private const string ResourcesPoisPath = "sample_pois"; // -> Assets/Resources/sample_pois.json

        public void LoadPOIs()
        {
            // 1) Repo /shared path (same level as Assets folder) OR override path
            string sharedPath = string.IsNullOrWhiteSpace(overrideSharedPath)
                ? Path.GetFullPath(Path.Combine(Application.dataPath, "..", "shared", "sample_pois.json"))
                : overrideSharedPath;

            Debug.Log("[POI] sharedPath=" + sharedPath);
            Debug.Log("[POI] shared exists=" + File.Exists(sharedPath));

            if (TryLoadFromFile(sharedPath, out var listFromFile))
            {
                LoadedPOIs = listFromFile;
                Debug.Log($"[POI] Loaded {LoadedPOIs.pois.Count} POIs from shared file.");
                return;
            }

            // 2) Resources fallback (Assets/Resources/sample_pois.json)
            if (TryLoadFromResources(ResourcesPoisPath, out var listFromResources))
            {
                LoadedPOIs = listFromResources;
                Debug.Log($"[POI] Loaded {LoadedPOIs.pois.Count} POIs from Resources/{ResourcesPoisPath}.json");
                return;
            }

            // 3) Hard fallback
            Debug.LogWarning("[POI] Could not load sample_pois.json from shared/ or Resources. Using fallback POIs.");
            LoadedPOIs = new POIList
            {
                pois = new List<POI>
                {
                    new POI { id = "fallback-1", name = "Fallback Rift", lat = 37.7749, lon = -122.4194, seed = 1234 }
                }
            };
        }

        private static bool TryLoadFromFile(string path, out POIList list)
        {
            list = null;

            try
            {
                if (!File.Exists(path)) return false;

                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json)) return false;

                list = JsonUtility.FromJson<POIList>(json);
                if (list == null || list.pois == null) return false;

                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[POI] Failed to load from file: " + path + "\n" + e);
                return false;
            }
        }

        private static bool TryLoadFromResources(string resourcesPathNoExt, out POIList list)
        {
            list = null;

            try
            {
                TextAsset asset = Resources.Load<TextAsset>(resourcesPathNoExt);
                if (asset == null)
                {
                    Debug.Log("[POI] Resources.Load returned null for: " + resourcesPathNoExt);
                    return false;
                }

                var json = asset.text;
                if (string.IsNullOrWhiteSpace(json)) return false;

                list = JsonUtility.FromJson<POIList>(json);
                if (list == null || list.pois == null) return false;

                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[POI] Failed to load from Resources: " + resourcesPathNoExt + "\n" + e);
                return false;
            }
        }

        public List<POI> GetNearest(double lat, double lon, int max = 5)
        {
            var results = new List<POI>(LoadedPOIs.pois);
            results.Sort((a, b) => DistanceMeters(lat, lon, a.lat, a.lon)
                .CompareTo(DistanceMeters(lat, lon, b.lat, b.lon)));

            if (results.Count > max)
                results = results.GetRange(0, max);

            return results;
        }

        public static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double radius = 6371000;
            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return radius * c;
        }

        private static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180.0);
    }
}

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

        public void LoadPOIs()
        {
            var sharedPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "shared", "sample_pois.json"));
            if (File.Exists(sharedPath))
            {
                var json = File.ReadAllText(sharedPath);
                LoadedPOIs = JsonUtility.FromJson<POIList>(json) ?? new POIList();
                return;
            }

            Debug.LogWarning("sample_pois.json not found, loading fallback POIs.");
            LoadedPOIs = new POIList
            {
                pois = new List<POI>
                {
                    new POI { id = "fallback-1", name = "Fallback Rift", lat = 37.7749, lon = -122.4194, seed = 1234 }
                }
            };
        }

        public List<POI> GetNearest(double lat, double lon, int max = 5)
        {
            var results = new List<POI>(LoadedPOIs.pois);
            results.Sort((a, b) => DistanceMeters(lat, lon, a.lat, a.lon).CompareTo(DistanceMeters(lat, lon, b.lat, b.lon)));
            if (results.Count > max)
            {
                results = results.GetRange(0, max);
            }
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

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }
    }
}

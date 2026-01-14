using UnityEngine;

namespace Worldrift.Client
{
    public class GeoLocalizer : MonoBehaviour
    {
        [Header("Scaling")]
        [SerializeField] private float metersToUnits = 0.05f;

        public bool HasOrigin { get; private set; }
        public double OriginLatitude { get; private set; }
        public double OriginLongitude { get; private set; }
        public double MetersPerDegreeLat { get; private set; }
        public double MetersPerDegreeLon { get; private set; }

        public float MetersToUnits => metersToUnits;

        public bool TrySetOrigin(double latitude, double longitude)
        {
            if (HasOrigin)
            {
                return false;
            }

            OriginLatitude = latitude;
            OriginLongitude = longitude;
            MetersPerDegreeLat = 111320.0;
            MetersPerDegreeLon = 111320.0 * Mathf.Cos((float)(OriginLatitude * Mathf.Deg2Rad));
            HasOrigin = true;
            return true;
        }

        public Vector2 LatLonToLocalMeters(double latitude, double longitude)
        {
            if (!HasOrigin)
            {
                return Vector2.zero;
            }

            double deltaLat = latitude - OriginLatitude;
            double deltaLon = longitude - OriginLongitude;

            float xMeters = (float)(deltaLon * MetersPerDegreeLon);
            float yMeters = (float)(deltaLat * MetersPerDegreeLat);

            return new Vector2(xMeters, yMeters);
        }

        public Vector2 LatLonToWorldUnits(double latitude, double longitude)
        {
            Vector2 meters = LatLonToLocalMeters(latitude, longitude);
            return MetersToWorldUnits(meters);
        }

        public Vector2 MetersToWorldUnits(Vector2 meters)
        {
            return meters * metersToUnits;
        }
    }
}

// Assumptions & Scene Wiring:
// - Add this component to a GameObject in the World scene (e.g., "GeoLocalizer").
// - WorldController should reference this component to convert GPS lat/lon to local world units.
// - metersToUnits can be tweaked in the Inspector (default 0.05).

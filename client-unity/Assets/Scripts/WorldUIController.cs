using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Worldrift.Client
{
    public class WorldUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text poiListText;
        [SerializeField] private TMP_Text statusText;

        public event Action<POI> PoiSelected;

        private List<POI> currentPois = new List<POI>();

        public void SetPois(List<POI> pois, double playerLat, double playerLon)
        {
            currentPois = pois;
            if (poiListText == null)
            {
                return;
            }

            var lines = new System.Text.StringBuilder();
            for (int i = 0; i < currentPois.Count; i++)
            {
                var poi = currentPois[i];
                var distance = WorldPOIService.DistanceMeters(playerLat, playerLon, poi.lat, poi.lon);
                lines.AppendLine($"[{i + 1}] {poi.name} - {distance:0}m");
            }

            poiListText.text = lines.ToString();
        }

        public void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        public void SelectPoiByIndex(int index)
        {
            if (index < 0 || index >= currentPois.Count)
            {
                SetStatus("Invalid POI selection");
                return;
            }

            PoiSelected?.Invoke(currentPois[index]);
        }
    }
}

using UnityEngine;

namespace Worldrift.Client
{
    public class InstanceController : MonoBehaviour
    {
        [SerializeField] private ColyseusClientManager colyseusManager;
        [SerializeField] private PlayerInputController inputController;
        [SerializeField] private InstanceUIController uiController;

        private AppConfig config;

        private async void Start()
        {
            config = AppConfig.Load();

            var playerName = PlayerPrefs.GetString("playerName", "Rift-Player");
            var faction = PlayerPrefs.GetString("faction", "Meridian Covenant");
            var poiId = PlayerPrefs.GetString("poiId", "fallback-1");
            var lat = PlayerPrefs.GetFloat("poiLat", 0f);
            var lon = PlayerPrefs.GetFloat("poiLon", 0f);

            uiController.SetStatus("Joining instance...");
            await colyseusManager.JoinInstanceAsync(playerName, faction, poiId, lat, lon);
            uiController.SetStatus("Connected");
        }

        private void Update()
        {
            if (inputController == null)
            {
                return;
            }

            colyseusManager.SendInput(inputController.CurrentInput);
        }
    }
}

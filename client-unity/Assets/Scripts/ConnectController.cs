using System.Threading.Tasks;
using UnityEngine;

namespace Worldrift.Client
{
    public class ConnectController : MonoBehaviour
    {
        [SerializeField] private ConnectUIController uiController;
        [SerializeField] private ColyseusClientManager colyseusManager;
        [SerializeField] private SceneFlowController sceneFlow;

        private AppConfig config;

        private void Awake()
        {
            config = AppConfig.Load();
            colyseusManager.Initialize(config);
            uiController.ConnectRequested += OnConnectRequested;
        }

        private async void OnConnectRequested(string playerName, string faction)
        {
            await colyseusManager.ConnectAsync();
            PlayerPrefs.SetString("playerName", playerName);
            PlayerPrefs.SetString("faction", faction);
            sceneFlow.GoToWorld();
        }
    }
}

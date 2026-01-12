using System;
using System.Threading.Tasks;
using UnityEngine;
using Colyseus;

namespace Worldrift.Client
{
    public class ColyseusClientManager : MonoBehaviour
    {
        public event Action Connected;
        public event Action Disconnected;
        public event Action JoinedRoom;

        private ColyseusClient client;
        private ColyseusRoom<State.RiftState> room;
        private AppConfig config;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Initialize(AppConfig appConfig)
        {
            config = appConfig;
        }

        public async Task ConnectAsync()
        {
            client = new ColyseusClient(config.serverEndpoint);
            Connected?.Invoke();
            await Task.Yield();
        }

        public async Task JoinInstanceAsync(string playerName, string faction, string poiId, double lat, double lon)
        {
            if (client == null)
            {
                throw new InvalidOperationException("Client not connected");
            }

            var options = new System.Collections.Generic.Dictionary<string, object>
            {
                { "name", playerName },
                { "faction", faction },
                { "poiId", poiId },
                { "lat", lat },
                { "lon", lon }
            };

            room = await client.JoinOrCreate<State.RiftState>("rift", options);
            JoinedRoom?.Invoke();
        }

        public void SendInput(Vector2 input)
        {
            if (room == null)
            {
                return;
            }

            room.Send("input", new { x = input.x, y = input.y });
        }

        public void Disconnect()
        {
            if (room != null)
            {
                room.Leave();
                room = null;
            }

            Disconnected?.Invoke();
        }
    }
}

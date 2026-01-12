using Colyseus.Schema;

namespace Worldrift.Client.State
{
    public class PlayerState : Schema
    {
        [Type(0, "string")] public string id = "";
        [Type(1, "string")] public string name = "";
        [Type(2, "string")] public string faction = "";
        [Type(3, "number")] public float x = 0f;
        [Type(4, "number")] public float y = 0f;
        [Type(5, "number")] public double lat = 0;
        [Type(6, "number")] public double lon = 0;
        [Type(7, "number")] public double lastSeen = 0;
    }

    public class RiftState : Schema
    {
        [Type(0, "map", typeof(MapSchema<PlayerState>))]
        public MapSchema<PlayerState> players = new MapSchema<PlayerState>();
    }
}

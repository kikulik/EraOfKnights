using UnityEngine;

namespace Worldrift.Client
{
    public class WorldChunkRenderer : MonoBehaviour
    {
        [Header("Chunk Settings")]
        [SerializeField] private int chunkSize = 64;
        [SerializeField] private int recenterThreshold = 8;
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private Transform tileParent;
        [SerializeField] private Sprite tileSprite;

        [Header("Biome Colors")]
        [SerializeField] private Color waterColor = new Color(0.2f, 0.4f, 0.8f, 1f);
        [SerializeField] private Color sandColor = new Color(0.85f, 0.8f, 0.5f, 1f);
        [SerializeField] private Color grassColor = new Color(0.2f, 0.7f, 0.3f, 1f);
        [SerializeField] private Color rockColor = new Color(0.5f, 0.5f, 0.55f, 1f);

        private SpriteRenderer[,] tiles;
        private int halfSize;
        private int centerTileX;
        private int centerTileY;
        private int seed;
        private bool initialized;
        private bool hasCenter;

        public void Initialize(double originLat, double originLon, int seedOffset)
        {
            seed = ComputeSeed(originLat, originLon, seedOffset);
            EnsureTiles();
            initialized = true;
        }

        public void UpdateChunk(Vector2 playerWorldUnits)
        {
            if (!initialized)
            {
                return;
            }

            int playerTileX = Mathf.FloorToInt(playerWorldUnits.x / tileSize);
            int playerTileY = Mathf.FloorToInt(playerWorldUnits.y / tileSize);

            if (tiles == null)
            {
                return;
            }

            if (!HasCenter())
            {
                centerTileX = playerTileX;
                centerTileY = playerTileY;
                hasCenter = true;
                RebuildTiles();
                return;
            }

            int dx = Mathf.Abs(playerTileX - centerTileX);
            int dy = Mathf.Abs(playerTileY - centerTileY);
            if (dx >= recenterThreshold || dy >= recenterThreshold)
            {
                centerTileX = playerTileX;
                centerTileY = playerTileY;
                RebuildTiles();
            }
        }

        private bool HasCenter()
        {
            return hasCenter;
        }

        private void EnsureTiles()
        {
            if (tileParent == null)
            {
                var parentObject = new GameObject("WorldChunk");
                parentObject.transform.SetParent(transform, false);
                tileParent = parentObject.transform;
            }

            if (tileSprite == null)
            {
                tileSprite = RuntimeSpriteFactory.CreateSquareSprite("TileSprite");
            }

            halfSize = chunkSize / 2;
            tiles = new SpriteRenderer[chunkSize, chunkSize];

            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    var tileObject = new GameObject($"Tile_{x}_{y}");
                    tileObject.transform.SetParent(tileParent, false);
                    var renderer = tileObject.AddComponent<SpriteRenderer>();
                    renderer.sprite = tileSprite;
                    renderer.sortingOrder = -10;
                    tiles[x, y] = renderer;
                }
            }
        }

        private void RebuildTiles()
        {
            if (tiles == null)
            {
                return;
            }

            for (int y = 0; y < chunkSize; y++)
            {
                int tileY = centerTileY + (y - halfSize);
                for (int x = 0; x < chunkSize; x++)
                {
                    int tileX = centerTileX + (x - halfSize);
                    var renderer = tiles[x, y];
                    renderer.transform.localPosition = new Vector3(tileX * tileSize, tileY * tileSize, 0f);
                    renderer.color = GetBiomeColor(tileX, tileY);
                }
            }
        }

        private Color GetBiomeColor(int tileX, int tileY)
        {
            float value = ValueNoise(tileX, tileY);
            if (value < 0.35f)
            {
                return waterColor;
            }
            if (value < 0.45f)
            {
                return sandColor;
            }
            if (value < 0.8f)
            {
                return grassColor;
            }
            return rockColor;
        }

        private float ValueNoise(int x, int y)
        {
            unchecked
            {
                int hash = x * 374761393 + y * 668265263;
                hash ^= seed;
                hash = (hash ^ (hash >> 13)) * 1274126177;
                hash ^= hash >> 16;
                return (hash & 0x7fffffff) / 2147483647f;
            }
        }

        private int ComputeSeed(double originLat, double originLon, int seedOffset)
        {
            unchecked
            {
                int latHash = Mathf.RoundToInt((float)(originLat * 10000));
                int lonHash = Mathf.RoundToInt((float)(originLon * 10000));
                int hash = latHash * 486187739;
                hash = (hash ^ lonHash) * 16777619;
                hash ^= seedOffset * 374761393;
                return hash;
            }
        }
    }

    internal static class RuntimeSpriteFactory
    {
        public static Sprite CreateSquareSprite(string name)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.name = name;
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        public static Sprite CreateCircleSprite(string name, int size = 32)
        {
            Texture2D texture = new Texture2D(size, size);
            texture.name = name;
            Color clear = new Color(0f, 0f, 0f, 0f);
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    texture.SetPixel(x, y, distance <= radius ? Color.white : clear);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }
    }
}

// Assumptions & Scene Wiring:
// - Add this component to a GameObject in the World scene (e.g., "WorldChunkRenderer").
// - Optionally assign a tileParent Transform; if empty a child named "WorldChunk" is created.
// - WorldController should call Initialize(originLat, originLon, seedOffset) and UpdateChunk(playerWorldUnits).
// - Adjust chunkSize/recenterThreshold/tileSize for different chunk behavior.

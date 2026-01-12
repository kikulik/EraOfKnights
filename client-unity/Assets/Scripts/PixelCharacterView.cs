using UnityEngine;

namespace Worldrift.Client
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PixelCharacterView : MonoBehaviour
    {
        [SerializeField] private Color playerColor = new Color(0.2f, 0.9f, 1f);

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = GenerateSprite(playerColor);
            }
        }

        public void SetColor(Color color)
        {
            playerColor = color;
            spriteRenderer.sprite = GenerateSprite(playerColor);
        }

        private Sprite GenerateSprite(Color color)
        {
            var texture = new Texture2D(16, 16)
            {
                filterMode = FilterMode.Point
            };
            var pixels = new Color[16 * 16];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16f);
        }
    }
}

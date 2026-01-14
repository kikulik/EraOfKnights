using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Worldrift.Client
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PortalMarker : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseAmplitude = 0.15f;

        private Vector3 baseScale;
        private TMP_Text tmpLabel;
        private Text uiLabel;

        public POI Poi { get; private set; }
        public event Action<POI> Clicked;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer != null && spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = RuntimeSpriteFactory.CreateCircleSprite("PortalSprite");
            }

            var collider = GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<CircleCollider2D>();
                collider.isTrigger = true;
            }

            baseScale = transform.localScale;
        }

        private void Update()
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
            transform.localScale = baseScale * pulse;
        }

        private void OnMouseDown()
        {
            Clicked?.Invoke(Poi);
        }

        public void Initialize(POI poi, string label, Color color)
        {
            Poi = poi;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
                spriteRenderer.sortingOrder = 1;
            }

            EnsureLabel(label);
        }

        private void EnsureLabel(string label)
        {
            if (tmpLabel == null && uiLabel == null)
            {
                var labelObject = new GameObject("PortalLabel");
                labelObject.transform.SetParent(transform, false);
                labelObject.transform.localPosition = new Vector3(0f, 0.7f, 0f);

                tmpLabel = labelObject.AddComponent<TextMeshPro>();
                if (tmpLabel != null)
                {
                    tmpLabel.alignment = TextAlignmentOptions.Center;
                    tmpLabel.fontSize = 2.5f;
                    tmpLabel.color = Color.white;
                    tmpLabel.enableAutoSizing = true;
                    tmpLabel.fontSizeMin = 1f;
                    tmpLabel.fontSizeMax = 2.5f;
                    tmpLabel.text = label;
                    return;
                }

                var canvas = labelObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                uiLabel = labelObject.AddComponent<Text>();
                uiLabel.alignment = TextAnchor.MiddleCenter;
                uiLabel.fontSize = 24;
                uiLabel.color = Color.white;
                uiLabel.text = label;
            }
            else
            {
                if (tmpLabel != null)
                {
                    tmpLabel.text = label;
                }
                else if (uiLabel != null)
                {
                    uiLabel.text = label;
                }
            }
        }
    }
}

// Assumptions & Scene Wiring:
// - Add this component to a GameObject (or prefab) with a SpriteRenderer for portal visuals.
// - WorldController instantiates/positions markers and subscribes to Clicked to select POIs.
// - Collider is auto-added for click detection; ensure a Camera exists in the scene.

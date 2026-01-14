using UnityEngine;

namespace Worldrift.Client
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAvatar : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float moveSpeed = 8f;

        private Vector3 targetPosition;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer != null && spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = RuntimeSpriteFactory.CreateCircleSprite("PlayerSprite");
                spriteRenderer.color = new Color(0.95f, 0.9f, 0.2f, 1f);
                spriteRenderer.sortingOrder = 2;
            }

            targetPosition = transform.position;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 1f - Mathf.Exp(-moveSpeed * Time.deltaTime));
        }

        public void SetWorldPosition(Vector2 worldUnits)
        {
            targetPosition = new Vector3(worldUnits.x, worldUnits.y, 0f);
        }
    }
}

// Assumptions & Scene Wiring:
// - Add this component to a GameObject (e.g., "PlayerAvatar") with a SpriteRenderer.
// - WorldController updates the player position via SetWorldPosition based on GPS local coords.
// - The sprite is auto-generated if none is assigned.

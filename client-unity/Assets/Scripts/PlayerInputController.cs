using UnityEngine;
using UnityEngine.InputSystem;

namespace Worldrift.Client
{
    public class PlayerInputController : MonoBehaviour
    {
        private InputAction moveAction;

        public Vector2 CurrentInput { get; private set; }

        private void Awake()
        {
            moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
        }

        private void OnEnable()
        {
            moveAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
        }

        private void Update()
        {
            CurrentInput = moveAction.ReadValue<Vector2>();
        }
    }
}

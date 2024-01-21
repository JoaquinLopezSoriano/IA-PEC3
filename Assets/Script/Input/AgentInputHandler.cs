using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Input
{
    public class AgentInputHandler : MonoBehaviour
    {
        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;

        [Tooltip("Additional sensitivity multiplier for WebGL")]
        public float WebglLookSensitivityMultiplier = 0.25f;

        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        public float TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;

        
        private AgentInput _agentInput;
        private InputAction _move;
        private InputAction _look;

        private void Awake()
        {
            _agentInput = new AgentInput();
            _agentInput.Enable();
            _move = _agentInput.Player.Move;
            _look = _agentInput.Player.Look;
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public Vector2 GetMoveInput()
        {
            return _move.ReadValue<Vector2>();
        }

        public Vector2 GetLookInput()
        {
            float i = 1;
            if (InvertYAxis)
                i *= -1f;

            // apply sensitivity multiplier
            return  _look.ReadValue<Vector2>() * i * LookSensitivity;
        }

    }
}
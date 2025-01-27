using UnityEngine;
using UnityEngine.InputSystem;

namespace ImprovedFirstPersonController 
{
    public class GetPlayerInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool attack;
        public bool interact;
        public bool jump;
        public bool sprint;
        public bool crouch;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;

        public void OnMove(InputValue value) {
            move = value.Get<Vector2>();
        }

        public void OnLook(InputValue value) {
            look = value.Get<Vector2>();
        }

        public void OnAttack(InputValue value) {
            attack = value.isPressed;
        }

        public void OnInteract(InputValue value) {
            interact = value.isPressed;
        }

        public void OnCrouch(InputValue value) {
            crouch = value.isPressed;
        }

        public void OnJump(InputValue value) {
            jump = value.isPressed;
        }

        public void OnSprint(InputValue value) {
            sprint = value.isPressed;
        }

        private void OnApplicationFocus(bool hasFocus) {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState) {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}

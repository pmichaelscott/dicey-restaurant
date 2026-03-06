using UnityEngine;
using UnityEngine.InputSystem;


public class PointerInput : MonoBehaviour
{
    public Vector2 PointerPosition { get; private set; }
    public bool PressedThisFrame { get; private set; }
    public bool ReleasedThisFrame { get; private set; }
    public bool IsPressed { get; private set; }

    [Header("Optional debug key")]
    [SerializeField] private Key rollKey = Key.R;

    public bool RollPressedThisFrame { get; private set; }

    private void Update()
    {
        // Reset one-frame flags
        PressedThisFrame = false;
        ReleasedThisFrame = false;
        RollPressedThisFrame = false;

        // Pointer
        var mouse = Mouse.current;
        var touch = Touchscreen.current;
        var pen = Pen.current;

        // Prefer touch if present and has touches, otherwise mouse, otherwise pen.
        if (touch != null && touch.primaryTouch.press.isPressed)
        {
            PointerPosition = touch.primaryTouch.position.ReadValue();

            bool nowPressed = touch.primaryTouch.press.wasPressedThisFrame;
            bool nowReleased = touch.primaryTouch.press.wasReleasedThisFrame;

            PressedThisFrame = nowPressed;
            ReleasedThisFrame = nowReleased;
            IsPressed = touch.primaryTouch.press.isPressed;
        }
        else if (mouse != null)
        {
            PointerPosition = mouse.position.ReadValue();

            PressedThisFrame = mouse.leftButton.wasPressedThisFrame;
            ReleasedThisFrame = mouse.leftButton.wasReleasedThisFrame;
            IsPressed = mouse.leftButton.isPressed;
        }
        else if (pen != null)
        {
            PointerPosition = pen.position.ReadValue();

            PressedThisFrame = pen.tip.wasPressedThisFrame;
            ReleasedThisFrame = pen.tip.wasReleasedThisFrame;
            IsPressed = pen.tip.isPressed;
        }

        // Optional keyboard key
        if (Keyboard.current != null)
            RollPressedThisFrame = Keyboard.current[rollKey].wasPressedThisFrame;
    }
}
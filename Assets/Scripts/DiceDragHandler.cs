using UnityEngine;

[RequireComponent(typeof(Dice))]
public class DiceDragHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private PointerInput input;
    [SerializeField] private DiceGameManager gameManager; // optional global gate

    [Header("Raycast")]
    [SerializeField] private LayerMask diceLayerMask = ~0;
    [SerializeField] private LayerMask slotLayerMask = ~0;

    [Header("Drag Plane")]
    [Tooltip("If true, drag plane is fixed at this Y. If false, plane is at dice's current Y when drag starts.")]
    [SerializeField] private bool useFixedPlaneY = false;
    [SerializeField] private float fixedPlaneY = 0f;

    [Header("Slot Behavior")]
    [SerializeField] private bool keepKinematicWhenSlotted = true;

    [Header("Drag Height")]
    [SerializeField] private float tableY = 0f;          // set to your tray/slot surface Y
    [SerializeField] private float hover = 0.01f;         // tiny lift to avoid z-fighting
    private float dragY;

    private Dice dice;
    private bool dragging = false;
    private Vector3 grabOffset = Vector3.zero;
    private Plane dragPlane;
    private DiceSlot currentSlot;

    private void Awake()
    {
        dice = GetComponent<Dice>();
        if (cam == null) cam = Camera.main;

        // You can place one PointerInput in the scene and reference it,
        // or as a fallback we'll try to find it.
        if (input == null) input = FindFirstObjectByType<PointerInput>();
    }

    private void Update()
    {
        if (input == null || cam == null)
            return;

        if (input.PressedThisFrame)
            TryBeginDrag();

        if (dragging && input.IsPressed)
            DragUpdate();

        if (dragging && input.ReleasedThisFrame)
            EndDrag();
    }

    private void TryBeginDrag()
    {
        // Global gate (optional)
        if (gameManager != null && !gameManager.CanInteract)
            return;

        // Per-die gate
        if (!dice.isInteractable)
            return;

        Ray ray = cam.ScreenPointToRay(input.PointerPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, diceLayerMask))
        {
            if (hit.collider != dice.col)
                return;

            dragging = true;
            dice.SetState(DiceState.Dragging);

            // Stop physics fighting while dragging
            dice.rb.isKinematic = true;

            float halfHeight = dice.col.bounds.extents.y;
            dragY = tableY + halfHeight + hover;

            // Only keep XZ offset (prevents sinking)
            grabOffset.y = 0f;

            // Set drag plane
            float y = useFixedPlaneY ? fixedPlaneY : transform.position.y;
            dragPlane = new Plane(Vector3.up, new Vector3(0f, y, 0f));

            // Keep the initial offset so the die doesn't jump
            grabOffset = transform.position - hit.point;

   
        }
    }

    private void DragUpdate()
    {
        Ray ray = cam.ScreenPointToRay(input.PointerPosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 point = ray.GetPoint(enter);
            Vector3 target = point + grabOffset;
            target.y = dragY;              // lock height
            transform.position = target;
        }

        UpdateCurrentSlot();
    }

    private void UpdateCurrentSlot()
    {
        // Raycast down from dice to see if a slot is underneath
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down,
                out RaycastHit hit, 3f, slotLayerMask))
        {
            currentSlot = hit.collider.GetComponentInParent<DiceSlot>();
        }
        else
        {
            currentSlot = null;
        }
    }

    private void EndDrag()
    {
        dragging = false;

        if (currentSlot != null && currentSlot.CanAccept(dice))
        {
            SnapIntoSlot(currentSlot);
            return;
        }

        // Drop / return to idle
        dice.SetState(DiceState.Idle);
        dice.rb.isKinematic = false;
    }

    private void SnapIntoSlot(DiceSlot slot)
    {
        Transform sp = slot.snapPoint;
        transform.SetPositionAndRotation(sp.position, sp.rotation);

        slot.Occupy(dice);
        dice.SetState(DiceState.Slotted);

        dice.rb.isKinematic = keepKinematicWhenSlotted;
    }
}
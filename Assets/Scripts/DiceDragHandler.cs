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
    

    [Header("Slot Behavior")]
    [SerializeField] private bool keepKinematicWhenSlotted = true;

    
    [Header("Drag Surface")]
    [SerializeField] private LayerMask dragSurfaceMask;
    [SerializeField] private float hoverHeight = 0.01f;
    [SerializeField] private bool useXZOffset = false;

    private Vector3 planarGrabOffset = Vector3.zero;

    private Dice dice;
    private bool dragging = false;
    
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
     if (gameManager != null && !gameManager.CanInteract)
        return;

    if (!dice.isInteractable)
        return;

    Ray ray = cam.ScreenPointToRay(input.PointerPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, diceLayerMask))
        {
            if (hit.collider != dice.col)
                return;

            if (dice.CurrentSlot != null)
            {
                dice.CurrentSlot.Vacate(dice);
                dice.CurrentSlot = null;
            }

            dragging = true;
            dice.SetState(DiceState.Dragging);
            dice.rb.isKinematic = true;

            // Optional: keep only XZ offset
            planarGrabOffset = transform.position - hit.point;
            planarGrabOffset.y = 0f;
        }
    }

    private void DragUpdate()
    {
    Ray ray = cam.ScreenPointToRay(input.PointerPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 500f, dragSurfaceMask))
            {
                float halfHeight = dice.col.bounds.extents.y;

                Vector3 target = hit.point + Vector3.up * (halfHeight + hoverHeight);

                if (useXZOffset)
                {
                    target.x += planarGrabOffset.x;
                    target.z += planarGrabOffset.z;
                }

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
        Transform sp = slot.SnapPoint;
        transform.position = sp.position;
        

        slot.Occupy(dice);
        dice.CurrentSlot = slot;
        dice.SetState(DiceState.Slotted);
        dice.rb.isKinematic = keepKinematicWhenSlotted;
    }
}
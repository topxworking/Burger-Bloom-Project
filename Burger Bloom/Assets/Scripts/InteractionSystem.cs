using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [Header("Settings")]
    public Transform holdPoint;
    public float interactRange = 2.5f;
    public float throwForce = 8f;
    public LayerMask interactLayer;

    private GameInputs input;
    private IPickable heldObject;

    void Awake() => input = new GameInputs();
    void OnEnable()
    {
        input.Enable();
        input.Player.Interact.performed += _ => OnInteract();
        input.Player.Throw.performed += _ => OnThrow();
        input.Player.Drop.performed += _ => OnDrop();
    }
    void OnDisable()
    {
        input.Disable();
        input.Player.Interact.performed -= _ => OnInteract();
        input.Player.Throw.performed -= _ => OnThrow();
        input.Player.Drop.performed -= _ => OnDrop();
    }

    void OnInteract()
    {
        if (heldObject is SauceBottle sauce)
        {
            if (Raycast(out RaycastHit hit) &&
                hit.collider.TryGetComponent(out BurgerStack burger))
            {
                burger.ApplySauce(sauce.sauceType);
            }
            return;
        }

        if (heldObject == null)
        {
            if (Raycast(out RaycastHit hit) &&
                hit.collider.TryGetComponent(out IPickable pickable) && pickable.CanPickup)
            {
                heldObject = pickable;
                heldObject.OnPickup(holdPoint);
            }
        }
    }

    void OnThrow()
    {
        if (heldObject == null) return;

        Camera cam = Camera.main;
        Vector3 dir = cam.transform.forward;
        heldObject.OnThrow(dir * throwForce);
        heldObject = null;
    }

    bool Raycast(out RaycastHit hit)
    {
        Camera cam = Camera.main;
        return Physics.Raycast(cam.transform.position, cam.transform.forward,
                       out hit, interactRange, interactLayer,
                       QueryTriggerInteraction.Collide);
    }

    void OnDrop()
    {
        if (heldObject == null) return;
        heldObject.OnDrop();
        heldObject = null;
    }

    public bool IsHolding() => heldObject != null;
}

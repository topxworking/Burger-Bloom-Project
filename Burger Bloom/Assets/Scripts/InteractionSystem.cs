using TMPro;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [Header("Settings")]
    public Transform holdPoint;
    public float interactRange = 2.5f;
    public float throwForce = 8f;
    public LayerMask interactLayer;
    public LayerMask blockLayer;

    private GameInputs input;
    private IPickable heldObject;
    private IInteractable lastInteractable;
    private IInteractable currentInteractable;

    void Awake() => input = new GameInputs();

    private void Start()
    {
        OnInteract();
    }

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

    void Update()
    {
        UpdateWorldPrompt();
    }

    void OnInteract()
    {
        if (Raycast(out RaycastHit hit2))
        {
            // ShopSign
            if (hit2.collider.TryGetComponent(out ShopSign sign))
            {
                sign.Interact();
                return;
            }
            // iPad
            if (hit2.collider.TryGetComponent(out iPadInteractable iPad))
            {
                iPad.Interact();
                return;
            }
        }

        // Sauce
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
                HoldingHUD.Instance?.ShowHolding(heldObject);
            }
        }
    }

    void OnThrow()
    {
        if (heldObject == null) return;

        SoundManager.Instance.PlayThrow();

        Camera cam = Camera.main;
        Vector3 dir = cam.transform.forward;
        heldObject.OnThrow(dir * throwForce);
        heldObject = null;
        HoldingHUD.Instance?.Hide();
    }

    bool Raycast(out RaycastHit hit)
    {
        Camera cam = Camera.main;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        LayerMask combinedMask = interactLayer | blockLayer;

        if (Physics.Raycast(ray, out hit, interactRange, combinedMask,
                            QueryTriggerInteraction.Collide))
        {
            if ((blockLayer & (1 << hit.collider.gameObject.layer)) != 0)
                return false;

            return true;
        }

        return false;
    }

    void OnDrop()
    {
        if (heldObject == null) return;
        heldObject.OnDrop();
        heldObject = null;
        HoldingHUD.Instance?.Hide();
    }

    public bool IsHolding() => heldObject != null;

    public void ForcePickup(IPickable pickable)
    {
        if (heldObject != null) return;
        heldObject = pickable;
        heldObject.OnPickup(holdPoint);
        HoldingHUD.Instance?.ShowHolding(heldObject);
    }

    void UpdateWorldPrompt()
    {
        Camera cam = Camera.main;
        currentInteractable = null;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward,
                        out RaycastHit hit, interactRange, interactLayer,
                        QueryTriggerInteraction.Collide))
        {
            if ((blockLayer & (1 << hit.collider.gameObject.layer)) != 0)
            {
                HideLastPrompt();
                return;
            }

            if (hit.collider != null && hit.collider.gameObject != null)
                currentInteractable = hit.collider.GetComponentInParent<IInteractable>();
        }

        if (currentInteractable != lastInteractable)
        {
            if (lastInteractable != null)
            {
                try { lastInteractable.UIPrompt?.Hide(); }
                catch { }
            }

            if (currentInteractable != null)
                currentInteractable.UIPrompt?.Show($"[E] {currentInteractable.InteractPrompt}");

            lastInteractable = currentInteractable;
        }
        else if (currentInteractable == null)
        {
            HideLastPrompt();
        }
    }

    void HideLastPrompt()
    {
        if (lastInteractable != null)
        {
            try { lastInteractable.UIPrompt?.Hide(); }
            catch { }
            lastInteractable = null;
        }
    }
}

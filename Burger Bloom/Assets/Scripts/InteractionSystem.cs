using TMPro;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [Header("Settings")]
    public Transform holdPoint;
    public float interactRange = 2.5f;
    public float throwForce = 8f;
    public LayerMask interactLayer;

    [Header("Interaction Prompt")]
    public TextMeshProUGUI promptText;

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

    private void Update()
    {
        if (Raycast(out RaycastHit hit) &&
        hit.collider.TryGetComponent(out iPadInteractable _))
        {
            promptText.text = "Press E to Upgrade";
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }

    void OnInteract()
    {
        if (Raycast(out RaycastHit iPadHit) &&
        iPadHit.collider.TryGetComponent(out iPadInteractable iPad))
        {
            iPad.Interact();
            return;
        }

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

        SoundManager.Instance.PlayThrow();

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

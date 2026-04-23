using UnityEngine;
using static Unity.VisualScripting.Member;

public class BurgerStack : MonoBehaviour, IPickable, IInteractable
{
    [Header("Snap")]
    public Transform meatSnapPoint;
    public Transform sauceVisual;
    public Transform bunTop;

    [Header("BunTop Lift")]
    public float bunTopClosedY = 0.15f;
    public float bunTopOpenY = 0.4f;
    public float liftSpeed = 3f;

    [Header("Sauce Materials")]
    public Material tomatoMaterial;
    public Material chiliMaterial;
    public Material mustardMaterial;

    public bool HasMeat { get; private set; }
    public SauceType? AppliedSauce { get; private set; } = null;
    public MeatType MeatType { get; private set; }
    public bool HasSauce => AppliedSauce.HasValue;
    public bool IsComplete => HasMeat;

    private Rigidbody rb;
    private Collider col;
    private bool isHeld;
    private bool isLifting;

    public bool CanPickup => !isHeld;
    public bool IsHeld() => isHeld;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        if (sauceVisual) sauceVisual.gameObject.SetActive(false);

        if (bunTop)
        {
            Vector3 pos = bunTop.localPosition;
            pos.y = bunTopClosedY;
            bunTop.localPosition = pos;
        }
    }

    void Update()
    {
        if (!isLifting || bunTop == null) return;
        Vector3 pos = bunTop.localPosition;
        float targetY = HasMeat ? bunTopOpenY : bunTopClosedY;
        pos.y = Mathf.MoveTowards(pos.y, targetY, liftSpeed * Time.deltaTime);
        bunTop.localPosition = pos;
        if (Mathf.Approximately(pos.y, targetY)) isLifting = false;
    }

    private void FixedUpdate()
    {
        if (isHeld) return;
        if (!HasMeat) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Customer"))
            {
                var customer = hit.GetComponentInParent<Customer>();
                if (customer != null)
                {
                    customer.ReceiveBurger(this);
                    return;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (HasMeat) return;
        if (isHeld) return;
        if (!other.TryGetComponent(out Ingredient ing)) return;
        if (ing.ingredientType != IngredientType.Meat) return;
        if (ing.cookState != CookState.Cooked) return;

        if (ing.IsOnGrill)
        {
            NotificationManager.Instance.Show("Meat is still cooking!");
            return;
        }

        if (ing.cookState != CookState.Cooked)
        {
            if (ing.cookState == CookState.Burnt)
                NotificationManager.Instance.Show("Meat is burnt!");
            return;
        }

        float dist = Vector3.Distance(transform.position, other.transform.position);
        if (dist > 0.3f) return;

        MeatType = ing.meatType;

        if (other.TryGetComponent(out Rigidbody meatRb))
        {
            meatRb.isKinematic = true;
            meatRb.useGravity = false;
        }

        other.enabled = false;
        other.transform.SetParent(transform, true);
        other.transform.position = meatSnapPoint.position;
        other.transform.rotation = meatSnapPoint.rotation;

        HasMeat = true;
        isLifting = true;

        SoundManager.Instance.PlayAssemble();
    }

    public void ApplySauce(SauceType sauce)
    {
        if (!HasMeat) return;
        if (HasSauce) return;

        AppliedSauce = sauce;
        SoundManager.Instance.PlaySauce();

        if (sauceVisual)
        {
            sauceVisual.gameObject.SetActive(true);
            var rend = sauceVisual.GetComponent<Renderer>();
            if (rend) rend.material = sauce switch
            {
                SauceType.Tomato => tomatoMaterial,
                SauceType.Chili => chiliMaterial,
                SauceType.Mustard => mustardMaterial,
                _ => rend.material
            };
        }
    }

    public void OnPickup(Transform holdPoint)
    {
        isHeld = true;
        rb.isKinematic = true;
        col.isTrigger = true;
        rb.useGravity = false;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnDrop()
    {
        isHeld = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        col.isTrigger = false;
        transform.SetParent(null);
    }

    public void OnThrow(Vector3 force)
    {
        OnDrop();
        rb.AddForce(force, ForceMode.Impulse);
    }

    public string InteractPrompt => HasMeat ? "Burger (Ready)" : "Burger Bun";
    public UIPrompt UIPrompt => this == null ? null : GetComponentInChildren<UIPrompt>(true);
}

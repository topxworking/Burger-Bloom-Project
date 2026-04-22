using UnityEngine;

public enum MeatType { Beef, Chicken }
public enum CookState { Raw, Cooked, Burnt }
public enum IngredientType { Meat, Bun, Sauce }

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Ingredient : MonoBehaviour, IPickable, IInteractable
{
    public IngredientType ingredientType;
    public MeatType meatType;
    public CookState cookState = CookState.Raw;

    [Header("Cook Materials")]
    public Material rawMaterial;
    public Material cookedMaterial;
    public Material burntMaterial;

    protected Rigidbody rb;
    protected Collider col;
    protected bool isHeld;

    public virtual bool CanPickup => !isHeld;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        ApplyMaterial();
    }

    public virtual void OnPickup(Transform holdPoint)
    {
        var grill = GetComponentInParent<GrillStation>();
        if (grill == null)
            grill = FindAnyObjectByType<GrillStation>();
        if (grill != null)
            grill.StopCooking(this);

        isHeld = true;
        rb.isKinematic = true;
        rb.useGravity = false;
        col.enabled = false;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public virtual void OnDrop()
    {
        isHeld = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        col.enabled = true;
        transform.SetParent(null);
    }

    public virtual void OnThrow(Vector3 force)
    {
        OnDrop();
        rb.AddForce(force, ForceMode.Impulse);
    }

    public void SetCookState(CookState state)
    {
        cookState = state;
        ApplyMaterial();
    }

    void ApplyMaterial()
    {
        if (!TryGetComponent(out Renderer r)) return;
        r.material = cookState switch
        {
            CookState.Raw => rawMaterial,
            CookState.Cooked => cookedMaterial,
            CookState.Burnt => burntMaterial,
            _ => rawMaterial
        };
    }

    public string InteractPrompt => ingredientType switch
    {
        IngredientType.Meat => meatType == MeatType.Beef ? "Beef Patty" : "Chicken Patty",
        IngredientType.Bun => "Burger Bun",
        _ => "Item"
    };

    public UIPrompt UIPrompt => GetComponentInChildren<UIPrompt>(true);
}

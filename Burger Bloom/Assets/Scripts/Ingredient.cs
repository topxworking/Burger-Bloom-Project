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

    [Header("VFX")]
    public ParticleSystem smokeVFX;

    protected Rigidbody rb;
    protected Collider col;
    protected bool isHeld;

    public virtual bool CanPickup => !isHeld;
    public bool IsHeld() => isHeld;
    public bool IsOnGrill { get; private set; }
    public void SetOnGrill(bool value) => IsOnGrill = value;

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
        UpdateSmokeVFX();
    }

    void UpdateSmokeVFX()
    {
        if (smokeVFX == null) return;

        switch (cookState)
        {
            case CookState.Raw:
                smokeVFX.Stop();
                break;

            case CookState.Cooked:
                var emission = smokeVFX.emission;
                emission.rateOverTime = 4f;
                if (!smokeVFX.isPlaying) smokeVFX.Play();
                break;

            case CookState.Burnt:
                var emissionBurnt = smokeVFX.emission;
                emissionBurnt.rateOverTime = 15f;
                var mainBurnt = smokeVFX.main;
                mainBurnt.startColor = new Color(0.1f, 0.1f, 0.1f, 0.6f);
                if (!smokeVFX.isPlaying) smokeVFX.Play();
                break;
        }
    }

    public void StartSmoke()
    {
        if (smokeVFX == null) return;
        var main = smokeVFX.main;
        main.startColor = new Color(0.8f, 0.85f, 1f, 0.4f);
        var emission = smokeVFX.emission;
        emission.rateOverTime = 8f;
        smokeVFX.Play();
    }

    public void StopSmoke()
    {
        if (smokeVFX == null) return;
        smokeVFX.Stop();
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

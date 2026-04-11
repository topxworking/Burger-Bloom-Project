using UnityEngine;

public enum IngredientType
{
    // Buns
    RegularBun, SesameBun, MholeMealBun, BriocheBun,
    // Proteins
    BeefPatty, ChickenPatty, FishFillet, VeggiePatty,
    // Vegetables
    Lettuce, Tomato, Onion, Pickle, Jalapeno,
    // Sauces
    Ketchup, Mustard, Mayo, BBQSauce, ChiliSauce, GarlicSauce,
    // Extras
    Cheese, BaconStrip, FriedEgg
}

[CreateAssetMenu(fileName = "Ingredient_", menuName = "ScriptableObjects/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    public IngredientType Type;
    public string DisplayName;
    [TextArea] public string Description;
    public Sprite Icon;
    public GameObject Prefab;
    public int UnlockLevel = 1;
    public float ShopPrice = 20f;
    public bool IsGrillable = false;
    public bool IsSauce = false;
    public Color SauceColor = Color.red;
}

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Ingredient : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientData _data;

    private Rigidbody _rb;
    private bool _isHeld;
    private bool _isPlaced;

    public string IngredientId => _data.Type.ToString();
    public IngredientType Type => _data.Type;
    public IngredientData Data => _data;
    public bool IsHeld => _isHeld;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public string GetPromptText()
    {
        return _isHeld ? "Drop [E]" : $"Pick up {_data.DisplayName} [E]";
    }

    public bool CanInteract(PlayerInteract player) => !_isPlaced;

    public void Interact(PlayerInteract player)
    {
        if (player.Hands.IsHolding && player.Hands.HeldIngredient == this)
            player.Hands.Drop();
        else
            player.Hands.PickUp(this);
    }

    public void OnPickedUp()
    {
        _isHeld = true;
        gameObject.layer = LayerMask.NameToLayer("HeldItem");
    }

    public void OnDropped()
    {
        _isHeld = false;
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public void OnPlaced()
    {
        _isHeld = false;
        _isPlaced = true;
        _rb.isKinematic = true;
    }

    public void SetData(IngredientData data) => _data = data;
}

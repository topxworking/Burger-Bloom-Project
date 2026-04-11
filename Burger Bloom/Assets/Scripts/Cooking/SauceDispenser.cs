using UnityEngine;

public enum SauceType
{
    Ketchup,
    Mustard,
    Mayo,
    BBQ,
    Chili,
    Garlic
}

public class SauceDispenser : MonoBehaviour, IInteractable
{
    [SerializeField] private SauceType _sauceType;
    [SerializeField] private Renderer _labelRenderer;

    private ISauceStrategy _strategy;

    private void Start()
    {
        _strategy = CreateStrategy(_sauceType);
        if (_labelRenderer != null)
            _labelRenderer.material.color = _strategy.Color;
    }

    private ISauceStrategy CreateStrategy(SauceType type) => type switch
    {
        SauceType.Ketchup => new KetchupSauce(),
        SauceType.Mustard => new MustardSauce(),
        SauceType.Mayo => new MayoSauce(),
        SauceType.BBQ => new BBQSauce(),
        SauceType.Chili => new ChiliSauce(),
        SauceType.Garlic => new GarlicSauce(),
        _ => new KetchupSauce()
    };

    public string GetPromptText() => $"Add {_strategy.Name} [E]";
    public bool CanInteract(PlayerInteract player) => true;

    public void Interact(PlayerInteract player)
    {
        if (player.Hands.HeldIngredient is BurgerAssemblyIngredient bai)
            _strategy.Apply(bai.Assembly);
    }
}

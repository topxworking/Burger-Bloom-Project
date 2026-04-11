using UnityEngine;

public interface ISauceStrategy
{
    string Name { get; }
    Color Color { get; }
    float FlavorMod { get; }
    void Apply(BurgerAssembly burger);
}

public class KetchupSauce : ISauceStrategy
{
    public string Name => "Ketchup";
    public Color Color => new Color(0.8f, 0.1f, 0.05f);
    public float FlavorMod => 1.0f;
    public void Apply(BurgerAssembly burger) => burger.AddSauce(this);
}

public class MustardSauce : ISauceStrategy
{
    public string Name => "Mustard";
    public Color Color => new Color(0.95f, 0.8f, 0.1f);
    public float FlavorMod => 0.9f;
    public void Apply(BurgerAssembly burger) => burger.AddSauce(this);
}

public class MayoSauce : ISauceStrategy
{
    public string Name => "Mayo";
    public Color Color => new Color(0.98f, 0.97f, 0.88f);
    public float FlavorMod => 1.0f;
    public void Apply(BurgerAssembly burger) => burger.AddSauce(this);
}

public class BBQSauce : ISauceStrategy
{
    public string Name => "BBQ";
    public Color Color => new Color(0.4f, 0.1f, 0.02f);
    public float FlavorMod => 1.2f;
    public void Apply(BurgerAssembly burger) => burger.AddSauce(this);
}

public class ChiliSauce : ISauceStrategy
{
    public string Name => "Chili";
    public Color Color => new Color(0.85f, 0.15f, 0f);
    public float FlavorMod => 1.15f;
    public void Apply(BurgerAssembly burger) => burger.AddSauce(this);
}

public class GarlicSauce : ISauceStrategy
{
    public string Name => "Garlic";
    public Color Color => new Color(0.9f, 0.9f, 0.75f);
    public float FlavorMod => 1.1f;
    public void Apply(BurgerAssembly burger) => burger.AddSauce(this);
}
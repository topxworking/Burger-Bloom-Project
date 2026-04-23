using UnityEngine;

public class CustomerDialogue : MonoBehaviour
{
    private static readonly string[] beefTomatoLines =
    {
        "One beef burger with ketchup, please!",
        "I'll take a classic beef with tomato sauce!",
        "Beef burger with ketchup, nice and simple.",
        "Can I get beef with tomato sauce?",
        "A beef burger, ketchup on top please!"
    };

    private static readonly string[] beefChiliLines =
    {
        "Give me a spicy beef burger!",
        "Beef with chili sauce - make it hot!",
        "I want that beef burger extra spicy!",
        "Chili beef burger, don't hold back!",
        "Hit me with a hot beef burger!"
    };

    private static readonly string[] beefMustardLines =
    {
        "Beef and mustard, the classic combo!",
        "I'd like a beef burger with mustard sauce.",
        "Make it beef with mustard, please.",
        "A mustard beef burger sounds perfect.",
        "Beef burger, mustard on it!"
    };

    private static readonly string[] chickenTomatoLines =
    {
        "Chicken burger with ketchup, thanks!",
        "Can I get a chicken with tomato sauce?",
        "Chicken and ketchup, please!",
        "I'll take a chicken burger with tomato.",
        "One chicken burger, ketchup on top!"
    };

    private static readonly string[] chickenChiliLines =
    {
        "Spicy chicken burger please!",
        "Chicken with chili - extra spicy!",
        "Give me a hot chicken burger!",
        "Chicken burger, make it spicy!",
        "I want chili chicken, nice and hot!"
    };

    private static readonly string[] chickenMustardLines =
    {
        "Chicken and mustard, sounds perfect!",
        "I'll have the chicken mustard burger.",
        "Chicken burger with mustard, please.",
        "Mustard chicken burger for me!",
        "Give me chicken with mustard sauce."
    };

    private static readonly string[] waitingLines =
    {
        "Hurry up, I'm starving!",
        "Is my order ready yet?",
        "I've been waiting forever...",
    };

    public string GetOrderLine(MeatType meat, SauceType sauce)
    {
        string[] lines = (meat, sauce) switch
        {
            (MeatType.Beef, SauceType.Tomato) => beefTomatoLines,
            (MeatType.Beef, SauceType.Chili) => beefChiliLines,
            (MeatType.Beef, SauceType.Mustard) => beefMustardLines,
            (MeatType.Chicken, SauceType.Tomato) => chickenTomatoLines,
            (MeatType.Chicken, SauceType.Chili) => chickenChiliLines,
            (MeatType.Chicken, SauceType.Mustard) => chickenMustardLines,
            _ => beefTomatoLines
        };

        return lines[Random.Range(0, lines.Length)];
    }

    public string GetWaitingLine()
        => waitingLines[Random.Range(0, waitingLines.Length)];
}
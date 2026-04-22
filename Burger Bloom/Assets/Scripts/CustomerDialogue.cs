using UnityEngine;

public class CustomerDialogue : MonoBehaviour
{
    private static readonly string[] beefTomatoLines =
    {
        "One beef burger with ketchup, please!",
        "I'll take a classic beef with tomato sauce!",
    };

    private static readonly string[] beefChiliLines =
    {
        "Give me a spicy beef burger!",
        "Beef with chili sauce - make it hot!",
    };

    private static readonly string[] beefMustardLines =
    {
        "Beef and mustard, the classic combo!",
        "I'd like a beef burger with mustard sauce.",
    };

    private static readonly string[] chickenTomatoLines =
    {
        "Chicken burger with ketchup, thanks!",
        "Can I get a chicken with tomato sauce?",
    };

    private static readonly string[] chickenChiliLines =
    {
        "Spicy chicken burger please!",
        "Chicken with chili - extra spicy!",
    };

    private static readonly string[] chickenMustardLines =
    {
        "Chicken and mustard, sounds perfect!",
        "I'll have the chicken mustard burger.",
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
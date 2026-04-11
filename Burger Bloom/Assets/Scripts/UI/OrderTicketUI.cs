using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderTicketUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _ingredientsText;
    [SerializeField] private TextMeshProUGUI _saucesText;
    [SerializeField] private Image _patienceBar;
    [SerializeField] private Image _ticketBg;

    [Header("Colors")]
    [SerializeField] private Color _urgentColor = new Color(0.9f, 0.2f, 0.1f);
    [SerializeField] private Color _normalColor = new Color(0.2f, 0.7f, 0.3f);

    private OrderData _order;
    private float _patienceMax;
    private float _patienceLeft;
    private bool _running;

    public void SetOrder(OrderData order)
    {
        _order = order;
        _patienceMax = order.Patience;
        _patienceLeft = order.Patience;
        _running = true;

        if (_headerText)
            _headerText.text = $"{order.Bun.ToString().Replace("Bun", "")} + {order.Protein.ToString().Replace("Patty", "")}";

        if (_ingredientsText)
        {
            var lines = new System.Text.StringBuilder();
            foreach (var t in order.RequiredIngredients)
                lines.AppendLine($"• {FormatIngredient(t)}");
            _ingredientsText.text = lines.ToString();
        }

        if (_saucesText)
        {
            _saucesText.text = order.RequiredSauces.Count > 0
                ? string.Join(", ", order.RequiredSauces)
                : "No sauce";
        }
    }

    private void Update()
    {
        if (!_running) return;

        _patienceLeft -= Time.deltaTime;
        float ratio = Mathf.Clamp01(_patienceLeft / _patienceMax);

        if (_patienceBar)
        {
            _patienceBar.fillAmount = ratio;
            _patienceBar.color = Color.Lerp(_urgentColor, _normalColor, ratio);
        }

        if (ratio < 0.2f && _ticketBg)
            _ticketBg.color = Color.Lerp(Color.white, _urgentColor,
                Mathf.PingPong(Time.time * 3f, 1f));

        if (_patienceLeft <= 0f) _running = false;
    }

    private string FormatIngredient(IngredientType t)
    {
        return t switch
        {
            IngredientType.Lettuce => "Lettuce",
            IngredientType.Tomato => "Tomato",
            IngredientType.Onion => "Onion",
            IngredientType.Pickle => "Pickle",
            IngredientType.Jalapeno => "Jalapeño",
            IngredientType.Cheese => "Cheese",
            IngredientType.BaconStrip => "Bacon",
            IngredientType.FriedEgg => "Egg",
            _ => t.ToString()
        };
    }
}

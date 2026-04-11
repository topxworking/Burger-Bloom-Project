using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemWidget : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private TextMeshProUGUI _stockText;

    private ShopItem _item;

    public void Setup(ShopItem item, System.Action<ShopItem> onBuy)
    {
        _item = item;
        if (_icon) _icon.sprite = item.Icon;
        if (_nameText) _nameText.text = item.DisplayName;
        if (_descText) _descText.text = item.Description;
        if (_priceText) _priceText.text = $"${item.PricePerBox:N0}";
        if (_stockText && !item.IsUpgrade)
        {
            int stock = InventorySystem.Instance.GetStock(item.IngredientType);
            _stockText.text = $"Stock: {stock}";
        }

        _buyButton.onClick.RemoveAllListeners();
        _buyButton.onClick.AddListener(() => onBuy(item));
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private List<ShopItem> _catalog;
    [SerializeField] private IngredientDatabase _database;
    [SerializeField] private DeliveryBox _deliveryBoxPrefab;
    [SerializeField] private Transform _deliveryPoint;

    [Header("UI Panels")]
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private Transform _itemListParent;
    [SerializeField] private ShopItemWidget _itemWidgetPrefab;

    [Header("Category Tabs")]
    [SerializeField] private Button _tabIngredients;
    [SerializeField] private Button _tabUpgrades;

    [Header("Info Panel")]
    [SerializeField] private TextMeshProUGUI _balanceText;
    [SerializeField] private TextMeshProUGUI _levelText;

    private ShopCategory _currentCategory = ShopCategory.Ingredients;
    private PlayerController _playerCtrl;
    private bool _isOpen;

    private void Start()
    {
        _shopPanel.SetActive(false);
        _tabIngredients.onClick.AddListener(() => SwitchCategory(ShopCategory.Ingredients));
        _tabUpgrades.onClick.AddListener(() => SwitchCategory(ShopCategory.Upgrades));

        EventBus.Subscribe<OnMoneyChanged>(OnMoneyChanged);
        EventBus.Subscribe<OnLevelUp>(OnLevelUp);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnMoneyChanged>(OnMoneyChanged);
        EventBus.Unsubscribe<OnLevelUp>(OnLevelUp);
    }

    public string GetPromptText() => "Open Shop [E]";
    public bool CanInteract(PlayerInteract player) => true;

    public void Interact(PlayerInteract player)
    {
        _playerCtrl = player.GetComponent<PlayerController>();
        Toggle();
    }

    private void Toggle()
    {
        _isOpen = !_isOpen;
        _shopPanel.SetActive(_isOpen);
        _playerCtrl?.SetLocked(_isOpen);

        if (_isOpen) Refresh();
    }

    public void Close()
    {
        _isOpen = false;
        _shopPanel.SetActive(false);
        _playerCtrl?.SetLocked(false);
    }

    private void SwitchCategory(ShopCategory cat)
    {
        _currentCategory = cat;
        Refresh();
    }

    private void Refresh()
    {
        foreach (Transform child in _itemListParent)
            Destroy(child.gameObject);

        int level = GameManager.Instance.Level;
        UpdateInfoPanel();

        foreach (var item in _catalog)
        {
            if (item.Category != _currentCategory) continue;
            if (item.UnlockLevel > level) continue;

            var widget = Instantiate(_itemWidgetPrefab, _itemListParent);
            widget.Setup(item, OnPurchase);
        }
    }

    private void UpdateInfoPanel()
    {
        if (_balanceText) _balanceText.text = $"?{GameManager.Instance.Money:N0}";
        if (_levelText) _levelText.text = $"Lv.{GameManager.Instance.Level}";
    }

    private void OnPurchase(ShopItem item)
    {
        if (item.IsUpgrade)
        {
            UpgradeSystem.Instance.Purchase(item);
            Refresh();
            return;
        }

        float cost = item.PricePerBox;
        if (!GameManager.Instance.SpendMoney(cost))
        {
            Debug.Log("[Shop] Not enough money!");
            return;
        }

        int units = item.UnitsPerBox + UpgradeSystem.Instance.DeliveryUnits - 10;
        DeliveryBox.Spawn(_deliveryBoxPrefab, item.IngredientType, units, _deliveryPoint.position);
        UpdateInfoPanel();
    }

    private void OnMoneyChanged(OnMoneyChanged _) => UpdateInfoPanel();
    private void OnLevelUp(OnLevelUp _) => Refresh();

    private void Update()
    {
        if (_isOpen && Input.GetKeyDown(KeyCode.Escape)) Close();
    }
}

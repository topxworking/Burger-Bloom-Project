using UnityEngine;
using TMPro;

public class ShopSign : MonoBehaviour, IInteractable
{
    [Header("Visual")]
    [SerializeField] private Renderer _signRenderer;
    [SerializeField] private TextMeshPro _signText;
    [SerializeField] private Color _openColor = new Color(0.2f, 0.85f, 0.3f);
    [SerializeField] private Color _closedColor = new Color(0.85f, 0.2f, 0.2f);
    [SerializeField] private Light _signLight;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    private void Start()
    {
        UpdateVisual();
        EventBus.Subscribe<OnShopOpened>(e => UpdateVisual());
        EventBus.Subscribe<OnShopClosed>(e => UpdateVisual());
    }

    public string GetPromptText()
    {
        return GameManager.Instance.State == GameState.DayPrepare
            ? "Open Shop [E]"
            : "Close Shop [E]";
    }

    public bool CanInteract(PlayerInteract player)
    {
        var s = GameManager.Instance.State;
        return s == GameState.DayPrepare || s == GameState.ShopOpen;
    }

    public void Interact(PlayerInteract player)
    {
        var state = GameManager.Instance.State;

        if (state == GameState.DayPrepare)
        {
            GameManager.Instance.OpenShop();
            _animator?.SetTrigger("Open");
        }
        else if (state == GameState.ShopOpen)
        {
            GameManager.Instance.CloseShop();
            _animator?.SetTrigger("Close");
        }
    }

    private void UpdateVisual()
    {
        bool isOpen = GameManager.Instance.State == GameState.ShopOpen;
        Color c = isOpen ? _openColor : _closedColor;

        if (_signRenderer)
            _signRenderer.material.color = c;

        if (_signText)
            _signText.text = isOpen ? "OPEN" : "CLOSED";

        if (_signLight)
        {
            _signLight.color = c;
            _signLight.intensity = isOpen ? 2f : 0.5f;
        }
    }
}

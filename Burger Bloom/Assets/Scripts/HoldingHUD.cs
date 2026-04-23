using UnityEngine;
using TMPro;

public class HoldingHUD : MonoBehaviour
{
    public static HoldingHUD Instance { get; private set; }

    [Header("UI")]
    public GameObject holdingPanel;
    public TextMeshProUGUI buttonsText;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        holdingPanel.SetActive(false);
    }

    public void ShowHolding(IPickable held)
    {
        if (held == null) { Hide(); return; }

        var mono = held as MonoBehaviour;
        if (mono == null) { Hide(); return; }

        holdingPanel.SetActive(true);

        if (mono.TryGetComponent(out SauceBottle _))
        {
            buttonsText.text = "[E] Pour Sauce\n[Q] Drop\n[F] Throw";
        }
        else if (mono.TryGetComponent(out BurgerStack _))
        {
            buttonsText.text = "[Q] Drop\n[F] Throw at Customer";
        }
        else
        {
            buttonsText.text = "[Q] Drop\n[F] Throw";
        }
    }

    public void Hide()
    {
        holdingPanel.SetActive(false);
    }
}

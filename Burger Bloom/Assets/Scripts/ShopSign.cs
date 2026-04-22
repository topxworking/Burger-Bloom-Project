using UnityEngine;

public class ShopSign : MonoBehaviour, IInteractable
{
    public Material openMaterial;
    public Material closedMaterial;
    public Renderer signRenderer;

    public void Interact()
    {
        if (!DayManager.Instance.IsOpen && !DayManager.Instance.DayEnded)
        {
            DayManager.Instance.OpenShop();
            signRenderer.material = openMaterial;
        }
        else if (DayManager.Instance.IsOpen)
        {
            DayManager.Instance.CloseShop();
            signRenderer.material = closedMaterial;
        }
    }

    public string InteractPrompt => DayManager.Instance.IsOpen ? "Close Shop" : "Open Shop";

    public UIPrompt UIPrompt => GetComponentInChildren<UIPrompt>(true);
}

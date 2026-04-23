using UnityEngine;

public class ShopSign : MonoBehaviour, IInteractable
{
    public Material openMaterial;
    public Material closedMaterial;
    public Renderer signRenderer;

    public void Interact()
    {
        if (DayManager.Instance.DayEnded) return;

        if (!DayManager.Instance.IsOpen)
        {
            // ปิดอยู่ > เปิดได้ถ้ายังไม่เลย 04:00 PM
            DayManager.Instance.OpenShop();
            signRenderer.material = openMaterial;
        }
        else
        {
            // เปิดอยู่ > ปิด
            DayManager.Instance.CloseShop();
            signRenderer.material = closedMaterial;
        }
    }

    public string InteractPrompt => DayManager.Instance.IsOpen ? "Close Shop"
                              : DayManager.Instance.DayEnded ? "Day Ended"
                              : "Open Shop";

    public UIPrompt UIPrompt => GetComponentInChildren<UIPrompt>(true);
}

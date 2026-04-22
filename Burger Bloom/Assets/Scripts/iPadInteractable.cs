using UnityEngine;

public class iPadInteractable : MonoBehaviour, IInteractable
{
    public UpgradeMenu upgradeMenu;

    public void Interact()
    {
        upgradeMenu.OpenMenu();
    }

    public string InteractPrompt => "Upgrade Shop";

    public UIPrompt UIPrompt => GetComponentInChildren<UIPrompt>(true);
}
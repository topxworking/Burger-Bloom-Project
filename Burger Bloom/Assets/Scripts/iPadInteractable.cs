using UnityEngine;

public class iPadInteractable : MonoBehaviour
{
    public UpgradeMenu upgradeMenu;

    public void Interact()
    {
        upgradeMenu.OpenMenu();
    }
}
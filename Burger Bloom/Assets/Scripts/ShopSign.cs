using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopSign : MonoBehaviour, IInteractable
{
    [Header("References")]
    public Renderer signRenderer;
    public Transform signTransform;

    [Header("Animation")]
    public float flipDuration = 0.4f;

    private bool isFlipping = false;
    private bool isOpen = false;

    public void Interact()
    {
        if (DayManager.Instance.DayEnded) return;
        if (isFlipping) return;

        if (!DayManager.Instance.IsOpen)
        {
            DayManager.Instance.OpenShop();
            StartCoroutine(FlipSign(true));
        }
        else
        {
            DayManager.Instance.CloseShop();
            StartCoroutine(FlipSign(false));
        }
    }

    public string InteractPrompt => DayManager.Instance.IsOpen ? "Close Shop"
                              : DayManager.Instance.DayEnded ? "Day Ended"
                              : "Open Shop";

    public UIPrompt UIPrompt => GetComponentInChildren<UIPrompt>(true);

    IEnumerator FlipSign(bool opening)
    {
        isFlipping = true;

        Transform t = signTransform != null ? signTransform : transform;
        float elapsed = 0f;

        Quaternion startRot = t.localRotation;
        Quaternion targetRot = startRot * Quaternion.Euler(0f, 180f, 0f);

        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;
            t.localRotation = Quaternion.Lerp(startRot, targetRot, elapsed / flipDuration);
            yield return null;
        }

        t.localRotation = targetRot;
        isOpen = opening;
        isFlipping = false;
    }

    public void RefreshMaterial(bool open)
    {
        if (signTransform == null) return;

        isOpen = open;
        Vector3 euler = signTransform.localEulerAngles;
        euler.y = open ? 180f : 0f;
        signTransform.localEulerAngles = euler;
    }
}

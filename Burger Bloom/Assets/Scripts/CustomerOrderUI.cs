using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomerOrderUI : MonoBehaviour
{
    public TextMeshPro dialogueText;
    public Transform playerCamera;
    public Image patienceFill;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (playerCamera == null) return;

        Vector3 dir = transform.position - playerCamera.position;
        dir.y = 0f;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    public void SetDialogue(string line)
    {
        if (dialogueText) dialogueText.text = $"\"{line}\"";
    }

    public void SetPatience(float normalized)
    {
        if (patienceFill != null)
            patienceFill.fillAmount = normalized;
    }
}
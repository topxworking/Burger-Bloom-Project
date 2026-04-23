using UnityEngine;
using TMPro;

public class CustomerOrderUI : MonoBehaviour
{
    public TextMeshPro dialogueText;
    public Transform playerCamera;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (playerCamera == null) return;
        transform.rotation = playerCamera.rotation;
    }

    public void SetDialogue(string line)
    {
        if (dialogueText) dialogueText.text = $"\"{line}\"";
    }
}
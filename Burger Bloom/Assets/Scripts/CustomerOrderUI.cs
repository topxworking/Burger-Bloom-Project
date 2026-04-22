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
        transform.LookAt(playerCamera);
        transform.Rotate(0f, 180f, 0f);
    }

    public void SetDialogue(string line)
    {
        if (dialogueText) dialogueText.text = $"\"{line}\"";
    }
}
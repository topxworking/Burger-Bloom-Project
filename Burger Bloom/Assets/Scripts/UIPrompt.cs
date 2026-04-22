using UnityEngine;
using TMPro;

public class UIPrompt : MonoBehaviour
{
    public TextMeshPro labelText;
    public Transform playerCamera;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main.transform;

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerCamera == null) return;

        Vector3 dir = transform.position - playerCamera.position;
        dir.y = 0f;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    public void Show(string text)
    {
        if (labelText == null)
        {
            Debug.LogError($"labelText is NULL on {gameObject.name}");
            return;
        }
        labelText.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

using UnityEngine;
using TMPro;

public class CustomerOrderUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshPro orderText;
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

    public void SetOrder(SauceType sauce)
    {
        orderText.text = sauce switch
        {
            SauceType.Tomato => "Tomato",
            SauceType.Chili => "Chili",
            SauceType.Mustard => "Mustard",
            _ => ""
        };
    }
}
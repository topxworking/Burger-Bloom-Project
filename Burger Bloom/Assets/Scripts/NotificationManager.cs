using System.Collections;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI notifyText;
    public float displayDuration = 2f;

    private Coroutine currentCoroutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        notifyText.gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ShowRoutine(message));
    }

    IEnumerator ShowRoutine(string message)
    {
        notifyText.text = message;
        notifyText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(displayDuration);
        notifyText.gameObject.SetActive(false);
    }
}

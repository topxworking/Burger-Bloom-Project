using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI")]
    public Slider progressBar;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI tipText;

    [Header("Settings")]
    public string gameSceneName = "GameScene";
    public float fakeDuration = 2.5f;

    private string[] tips =
    {
        "Cook meat until brown for best price!",
        "Wrong sauce = lower score. Check the order!",
        "Upgrade Cook Speed to serve faster.",
        "Stock up before 4:00 PM!",
        "Throw burgers accurately to serve customers.",
        "Open the shop early to maximize income."
    };

    void Start()
    {
        if (tipText) tipText.text = tips[Random.Range(0, tips.Length)];
        SetProgress(0f);
        StartCoroutine(LoadRoutine());
    }

    IEnumerator LoadRoutine()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(gameSceneName);
        op.allowSceneActivation = false;

        float elapsed = 0f;

        while (elapsed < fakeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fakeDuration);

            SetProgress(Mathf.Lerp(0f, 0.95f, t));
            UpdateText(t);

            yield return null;
        }

        SetProgress(1f);
        if (loadingText) loadingText.text = "Ready!";

        yield return new WaitForSeconds(0.4f);

        op.allowSceneActivation = true;
    }

    void SetProgress(float value)
    {
        if (progressBar == null) return;
        progressBar.value = value;
    }

    void UpdateText(float t)
    {
        if (loadingText == null) return;
        loadingText.text = t < 0.4f ? "Loading assets..."
                         : t < 0.75f ? "Setting up kitchen..."
                         : "Almost ready...";
    }
}
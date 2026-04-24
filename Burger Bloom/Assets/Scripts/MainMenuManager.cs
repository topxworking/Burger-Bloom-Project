using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button continueButton;
    public Button newGameButton;

    [Header("Confirm Panel")]
    public GameObject confirmPanel;
    public TextMeshProUGUI confirmText;

    [Header("Settings")]
    public string loadingSceneName = "LoadingScene";

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        RefreshButtons();
    }

    void RefreshButtons()
    {
        bool hasSave = PlayerPrefs.HasKey("BurgerBloom_Save");

        Debug.Log($"HasSave: {hasSave}");

        if (continueButton)
        {
            continueButton.interactable = hasSave;
            Debug.Log($"Continue interactable: {continueButton.interactable}");
        }

        if (confirmPanel) confirmPanel.SetActive(false);
    }

    public void OnContinue()
    {
        SceneManager.LoadScene(loadingSceneName);
    }

    public void OnNewGame()
    {
        bool hasSave = PlayerPrefs.HasKey("BurgerBloom_Save");

        if (hasSave)
        {
            if (confirmPanel)
            {
                confirmPanel.SetActive(true);
                if (confirmText)
                    confirmText.text = "Delete save and start over?";
            }
        }
        else
        {
            StartNewGame();
        }
    }

    public void OnConfirmNewGame()
    {
        PlayerPrefs.DeleteKey("BurgerBloom_Save");
        PlayerPrefs.Save();
        confirmPanel.SetActive(false);
        RefreshButtons();
        StartNewGame();
    }

    public void OnCancelNewGame()
    {
        if (confirmPanel) confirmPanel.SetActive(false);
    }

    void StartNewGame()
    {
        SceneManager.LoadScene(loadingSceneName);
    }

    public void OnQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

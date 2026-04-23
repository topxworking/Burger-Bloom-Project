using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("UI")]
    public GameObject pausePanel;
    public TextMeshProUGUI saveStatusText;

    private bool isPaused;
    private GameInputs input;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        input = new GameInputs();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.Pause.performed += _ => TogglePause();
    }

    void OnDisable()
    {
        input.Disable();
        input.Player.Pause.performed -= _ => TogglePause();
    }

    void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // แสดงสถานะ save
        saveStatusText.text = SaveManager.Instance.HasSave()
            ? "Last save exists"
            : "No save found";
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ปุ่ม Resume
    public void OnResume() => Resume();

    // ปุ่ม Save
    public void OnSave()
    {
        SaveManager.Instance.Save();
        saveStatusText.text = "Saved!";
    }

    // ปุ่ม Save & Quit
    public void OnSaveAndQuit()
    {
        SaveManager.Instance.Save();
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ปุ่ม Quit (ไม่ save)
    public void OnQuit()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

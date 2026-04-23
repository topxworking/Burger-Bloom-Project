using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SAVE_KEY = "BurgerBloom_Save";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Save()
    {
        var player = FindAnyObjectByType<PlayerController>();

        var data = new SaveData
        {
            money = GameManager.Instance.Money,
            currentDay = DayManager.Instance.CurrentDay,
            currentHour = DayManager.Instance.CurrentHour,
            isOpen = DayManager.Instance.IsOpen,
            dayStarted = DayManager.Instance.DayStarted,
            bunLevel = GameManager.Instance.upgradeData.bunLevel,
            meatLevel = GameManager.Instance.upgradeData.meatLevel,
            cookLevel = GameManager.Instance.upgradeData.cookLevel,
            burnLevel = GameManager.Instance.upgradeData.burnLevel,
            speedLevel = GameManager.Instance.upgradeData.speedLevel,
            beefStock = StockManager.Instance.beefStock,
            chickenStock = StockManager.Instance.chickenStock,
            bunStock = StockManager.Instance.bunStock,

            playerX = player != null ? player.transform.position.x : 0f,
            playerY = player != null ? player.transform.position.y : 0f,
            playerZ = player != null ? player.transform.position.z : 0f,
            playerRotY = player != null ? player.transform.eulerAngles.y : 0f,
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        NotificationManager.Instance.Show("Game Saved!");
    }

    public bool HasSave() => PlayerPrefs.HasKey(SAVE_KEY);

    public void Load()
    {
        if (!HasSave()) return;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // เงิน
        GameManager.Instance.SetMoney(data.money);

        // Upgrade
        var ud = GameManager.Instance.upgradeData;
        ud.bunLevel = data.bunLevel;
        ud.meatLevel = data.meatLevel;
        ud.cookLevel = data.cookLevel;
        ud.burnLevel = data.burnLevel;
        ud.speedLevel = data.speedLevel;

        // Stock
        StockManager.Instance.beefStock = data.beefStock;
        StockManager.Instance.chickenStock = data.chickenStock;
        StockManager.Instance.bunStock = data.bunStock;

        var player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            var cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;
            player.transform.position = new Vector3(data.playerX, data.playerY, data.playerZ);
            player.transform.eulerAngles = new Vector3(0f, data.playerRotY, 0f);
            if (cc) cc.enabled = true;
        }

        DayManager.Instance.LoadState(
            data.currentDay,
            data.currentHour,
            data.isOpen,
            data.dayStarted
        );

        // Apply upgrade effects
        var pc = FindAnyObjectByType<PlayerController>();
        if (pc) pc.walkSpeed = ud.GetWalkSpeed();

        var grill = FindAnyObjectByType<GrillStation>();
        if (grill) grill.RefreshCookTimes();
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        NotificationManager.Instance.Show("Save deleted!");
    }
}

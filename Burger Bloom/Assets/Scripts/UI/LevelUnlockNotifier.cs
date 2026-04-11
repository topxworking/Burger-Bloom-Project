using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUnlockNotifier : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject _popup;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _unlocksText;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _autoDismiss = 4f;

    [Header("Database")]
    [SerializeField] private IngredientDatabase _database;

    private int _previousLevel = 1;

    private void Start()
    {
        _popup.SetActive(false);
        EventBus.Subscribe<OnLevelUp>(OnLevelUp);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnLevelUp>(OnLevelUp);
    }

    private void OnLevelUp(OnLevelUp e)
    {
        int newLevel = e.NewLevel;

        var unlocked = new List<IngredientData>();
        if (_database != null)
        {
            foreach (IngredientType t in System.Enum.GetValues(typeof(IngredientType)))
            {
                var data = _database.Get(t);
                if (data != null && data.UnlockLevel == newLevel)
                    unlocked.Add(data);
            }
        }

        StartCoroutine(ShowPopup(newLevel, unlocked));
        _previousLevel = newLevel;
    }

    private IEnumerator ShowPopup(int level, List<IngredientData> unlocked)
    {
        _popup.SetActive(true);
        _animator?.SetTrigger("Show");

        if (_levelText)
            _levelText.text = $"Level {level}!";

        if (_unlocksText)
        {
            if (unlocked.Count > 0)
            {
                var sb = new System.Text.StringBuilder("Unlocked:\n");
                foreach (var d in unlocked)
                    sb.AppendLine($"{d.DisplayName}");
                _unlocksText.text = sb.ToString();
            }
            else
            {
                _unlocksText.text = "Keep cooking!";
            }
        }

        yield return new WaitForSeconds(_autoDismiss);
        _animator?.SetTrigger("Hide");
        yield return new WaitForSeconds(0.5f);
        _popup.SetActive(false);
    }
}

using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float money = 500f;
    public int level = 1;
    public int xp = 0;
    public int day = 1;
    public int[] stock;
}

public static class SaveSystem
{
    private const string KEY = "BurgerBloomSave";

    public static void Save(SaveData data)
    {
        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        if (!PlayerPrefs.HasKey(KEY)) return new SaveData();
        return JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(KEY));
    }

    public static void Delete() => PlayerPrefs.DeleteKey(KEY);
}

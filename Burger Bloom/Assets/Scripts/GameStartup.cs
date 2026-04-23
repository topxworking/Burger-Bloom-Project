using System.Collections;
using UnityEngine;

public class GameStartup : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;

        if (SaveManager.Instance.HasSave())
        {
            SaveManager.Instance.Load();
            NotificationManager.Instance.Show("Save loaded!");
        }
        else
        {
            NotificationManager.Instance.Show("New Game!");
        }
    }
}
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
            NotificationManager.Instance.Show("Welcome back!");
        }

        Application.targetFrameRate = -1;
    }
}
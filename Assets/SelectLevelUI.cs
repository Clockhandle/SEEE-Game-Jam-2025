using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SelectLevelUI : MonoBehaviour
{
    public Button[] button;
    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("unlockedLevel", 1);
        for (int i = 0; i < button.Length; i++)
        {
            button[i].interactable = false;

        }

        int maxUnlock = Mathf.Min(unlockedLevel, button.Length);

        for (int i = 0; i < maxUnlock; i++)
        {
            button[i].interactable = true;
            Debug.Log($"UnlockLevel number {i}");
        }
    }

    public void OpenLevel(int levelID)
    {
        string levelName = "Level" + levelID;
        SceneManager.LoadScene(levelName);
    }
}

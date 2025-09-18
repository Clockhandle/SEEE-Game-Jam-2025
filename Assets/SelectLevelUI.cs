using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SelectLevelUI : MonoBehaviour
{
    public Button[] button;
    private Image lockLevelImg;

    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("unlockedLevel", 1);
        for (int i = 0; i < button.Length; i++)
        {
            button[i].interactable = false;
            button[i].GetComponentInChildren<LockedImgReference>().gameObject.SetActive(true);
        }

        int maxUnlock = Mathf.Min(unlockedLevel, button.Length);

        for (int i = 0; i < maxUnlock; i++)
        {
            button[i].interactable = true;
            button[i].GetComponentInChildren<LockedImgReference>().gameObject.SetActive(false);
            Debug.Log($"UnlockLevel number {i}");
        }
    }

    public void OpenLevel(int levelID)
    {
        StartCoroutine(DelayTransis(levelID));
    }

    private IEnumerator DelayTransis(int levelID)
    {
        WhiteFlash.instance.ActiveFlashScreen();
        yield return new WaitForSeconds(1f);
        string levelName = "Level" + levelID;
        SceneManager.LoadScene(levelName);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    bool isGameOver;
    public Button unlockButton;



    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      
        unlockButton = FindObjectOfType<UnlockLVButton>()?.GetComponent<Button>();

        if (unlockButton != null)
        {
            unlockButton.onClick.RemoveAllListeners(); 
            unlockButton.onClick.AddListener(() =>
            {
                GameManager_OnLevelWin();
            });
        }
    }




    private void GameManager_OnLevelWin()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachIndex"))
        {
            PlayerPrefs.SetInt("ReachIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("unlockedLevel", PlayerPrefs.GetInt("unlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }



}

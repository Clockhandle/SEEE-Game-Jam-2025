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

    [Header("Auto Progression Settings")]
    [SerializeField] private bool autoProgressToNextLevel = true;
    [SerializeField] private float progressionDelay = 2f; // Delay after winning before transitioning
    
    // Add safeguard against multiple level completion saves
    private bool levelCompletionSaved = false;

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
        string sceneName = scene.name;
        
        // Reset level completion flag for new scene
        levelCompletionSaved = false;
        
        // Only set up UnlockLVButton functionality for actual level scenes, not for LevelSelect
        if (IsLevelScene(sceneName))
        {
            unlockButton = FindObjectOfType<UnlockLVButton>()?.GetComponentInChildren<Button>(true);

            if (unlockButton != null)
            {
                unlockButton.onClick.RemoveAllListeners(); 
                unlockButton.onClick.AddListener(() =>
                {
                    // Manual button click - just progress without updating (since auto-progression already did it)
                    FindObjectOfType<UnlockLVButton>().OpenSelectionLevel();
                });
            }

            // Subscribe to win event for automatic progression
            if (autoProgressToNextLevel && WinFlagGoal.instance != null)
            {
                WinFlagGoal.instance.OnTriggerWinFlag += OnLevelWin;
            }
        }
        else
        {
            Debug.Log($"Skipping GameManager setup for non-level scene: {sceneName}");
        }
    }

    private bool IsLevelScene(string sceneName)
    {
        // Check if this is a level scene (starts with "Demo_" or contains level indicators)
        return sceneName.StartsWith("Demo_");
    }

    private void OnLevelWin(object sender, EventArgs e)
    {
        // Prevent multiple level completion processing
        if (levelCompletionSaved)
        {
            Debug.Log("Level completion already processed, ignoring duplicate win event");
            return;
        }
        
        if (autoProgressToNextLevel)
        {
            StartCoroutine(AutoProgressToNextLevel());
        }
    }

    private IEnumerator AutoProgressToNextLevel()
    {
        // Wait for the specified delay (gives time for win animation/effects)
        yield return new WaitForSeconds(progressionDelay);
        
        // Update player progress
        GameManager_OnLevelWin();
        
        // Progress to next level using UnlockLVButton's logic
        UnlockLVButton unlockLVComponent = FindObjectOfType<UnlockLVButton>();
        if (unlockLVComponent != null)
        {
            unlockLVComponent.OpenSelectionLevel();
        }
    }

    private void GameManager_OnLevelWin()
    {
        // Prevent saving progress multiple times for the same level
        if (levelCompletionSaved)
        {
            Debug.Log("Level completion already saved, skipping duplicate save");
            return;
        }
        
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachIndex"))
        {
            PlayerPrefs.SetInt("ReachIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("unlockedLevel", PlayerPrefs.GetInt("unlockedLevel", 1) + 1);
            PlayerPrefs.Save();
            levelCompletionSaved = true; // Mark as saved
            Debug.Log($"Progress saved! Unlocked level: {PlayerPrefs.GetInt("unlockedLevel", 1)}");
        }
        else
        {
            Debug.Log("Level already completed previously, not updating progress");
        }
    }

    public void ReLoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent errors
        if (WinFlagGoal.instance != null)
        {
            WinFlagGoal.instance.OnTriggerWinFlag -= OnLevelWin;
        }
    }
}

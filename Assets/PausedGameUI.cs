using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausedGameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backToSelectorButton;
    
    [Header("Optional")]
    [SerializeField] private GameObject pauseMenuPanel; // Optional container for the pause menu

    private void Awake()
    {
        // Set up button listeners
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(() => {
                ContinueGame();
            });
        }

        if (backToSelectorButton != null)
        {
            backToSelectorButton.onClick.AddListener(() => {
                BackToLevelSelector();
            });
        }
    }

    private void Start()
    {
        // Hide pause menu at start
        Hide();
    }

    /// <summary>
    /// Call this to show the pause menu (from your pause button)
    /// </summary>
    public void ShowPauseMenu()
    {
        GameManager.Instance.PauseGame();
        Show();
    }

    /// <summary>
    /// Continue button functionality - resume the game
    /// </summary>
    private void ContinueGame()
    {
        Hide();
        GameManager.Instance.UnpauseGame();
    }

    /// <summary>
    /// Back to selector button functionality - unpause and go to level select
    /// </summary>
    private void BackToLevelSelector()
    {
        Hide();
        GameManager.Instance.UnpauseGame();
        
        // Add flash effect if available
        if (WhiteFlash.instance != null)
        {
            WhiteFlash.instance.ActiveFlashScreen();
            StartCoroutine(DelayedLevelSelect());
        }
        else
        {
            LoadLevelSelector();
        }
    }

    private IEnumerator DelayedLevelSelect()
    {
        yield return new WaitForSeconds(1f);
        LoadLevelSelector();
    }

    private void LoadLevelSelector()
    {
        LoadSceneManage.Load(LoadSceneManage.SpecialScene.LevelSelect);
    }

    /// <summary>
    /// Show the pause menu UI
    /// </summary>
    private void Show()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

        // Optional: Select the continue button for controller/keyboard navigation
        if (continueButton != null)
        {
            continueButton.Select();
        }
    }

    /// <summary>
    /// Hide the pause menu UI
    /// </summary>
    private void Hide()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Optional: Handle ESC key input for pause toggle
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuPanel != null ? pauseMenuPanel.activeSelf : gameObject.activeSelf)
            {
                ContinueGame(); // If menu is open, continue game
            }
            else
            {
                ShowPauseMenu(); // If menu is closed, show pause menu
            }
        }
    }
}

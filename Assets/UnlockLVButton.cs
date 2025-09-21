using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UnlockLVButton : MonoBehaviour
{
    public void OpenSelectionLevel()
    {
        StartCoroutine(DelayTransition());
    }

    private IEnumerator DelayTransition()
    {
        WhiteFlash.instance.ActiveFlashScreen();
        yield return new WaitForSeconds(1f);
        
        // Get current scene build index
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        
        // Check if there's a next scene
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Load next level
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            // No more levels, go to level selector
            LoadSceneManage.Load(LoadSceneManage.SpecialScene.LevelSelect);
        }
    }

    public void OpenMainMenu()
    {
        LoadSceneManage.Load(LoadSceneManage.SpecialScene.MainMenu2D);
    }
}

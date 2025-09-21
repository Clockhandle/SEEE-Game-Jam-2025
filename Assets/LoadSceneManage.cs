using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadSceneManage
{
    public enum SpecialScene
    {
        MainMenu2D,
        LoadingScene2D,
        LevelSelect,
        Level1,
        Level2
    }

    public static void LoadLevel(string sceneName)
    {
        // Load scene directly without loading screen
        SceneManager.LoadScene(sceneName);
    }

    public static void Load(SpecialScene specialScene)
    {
        // Load scene directly without loading screen
        SceneManager.LoadScene(specialScene.ToString());
    }

    // Keep this method for backwards compatibility, but it's no longer needed
    public static void LoaderCallback()
    {
        // This method is no longer used since we load scenes directly
        Debug.LogWarning("LoaderCallback() is deprecated. Scenes now load directly without loading screen.");
    }
}

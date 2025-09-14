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

    private static string targetScene;

  
    public static void LoadLevel(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.LoadScene(targetScene);
    }


    public static void Load(SpecialScene specialScene)
    {
        targetScene = specialScene.ToString();
        SceneManager.LoadScene(SpecialScene.LoadingScene2D.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnlockLVButton : MonoBehaviour
{

    public void OpenSelectionLevel()
    {
        StartCoroutine(DelayTransition());
    }


    private IEnumerator DelayTransition()
    {
        //yield return new WaitForSeconds(2f);
        yield return null;
        string levelName = "LevelSelect";
        SceneManager.LoadScene(levelName);
    }
    public void OpenMainMenu()
    {
        string levelName = "MainMenu2D";
        SceneManager.LoadScene(levelName);
    }
}

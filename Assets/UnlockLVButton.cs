using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UnlockLVButton : MonoBehaviour
{

    private void OnEnable()
    {
        
    }

    public void OpenSelectionLevel()
    {
        StartCoroutine(DelayTransition());
    }


    private IEnumerator DelayTransition()
    {
        WhiteFlash.instance.ActiveFlashScreen();
        yield return new WaitForSeconds(1f);
        //yield return null;
        string levelName = "LevelSelect";
        SceneManager.LoadScene(levelName);
    }
    public void OpenMainMenu()
    {
        string levelName = "MainMenu2D";
        SceneManager.LoadScene(levelName);
    }
}

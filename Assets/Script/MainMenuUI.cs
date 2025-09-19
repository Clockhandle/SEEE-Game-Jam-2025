using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            StartCoroutine(DelayTransis());
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        Time.timeScale = 1f;
    }

    IEnumerator DelayTransis()
    {
        WhiteFlash.instance.ActiveFlashScreen();
        yield return new WaitForSeconds(1f);
        LoadSceneManage.Load(LoadSceneManage.SpecialScene.LevelSelect);
    }
}

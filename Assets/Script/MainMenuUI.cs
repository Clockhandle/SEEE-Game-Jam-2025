using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    
    private void Awake()
    {
        playButton.onClick.AddListener(() => {
            ScreenWipe.TransitionToScene("LevelSelect");
        });
        quitButton.onClick.AddListener(() => Application.Quit());
        Time.timeScale = 1f;
    }
    
}

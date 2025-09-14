using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausedGameUI : MonoBehaviour
{
    //public event EventHandler OnPausedEvent;
    //public event EventHandler OnUnPausedEvent;

    //[SerializeField] private Button contitnueButton;
    //[SerializeField] private Button closeLevelButton;
    //[SerializeField] private ResultShowUI resultUI;
    //private void Awake()
    //{


    //    contitnueButton.onClick.AddListener(() => {
    //        GameManager.Instance.TogglePauseGame();
    //    });

    //    closeLevelButton.onClick.AddListener(() => {
    //        Hide();
    //        resultUI.gameObject.SetActive(true);
    //        Time.timeScale = 1f;
    //        //OptionsUI.Instance.Show(Show);
    //    });

       
    //}
    //private void Start()
    //{
    //    GameManager.Instance.OnGamePaused += GameManager_OnPausedAction;
    //    GameManager.Instance.OnGameUnPaused += GameManager_OnUnPausedAction;

    //    Hide();  //prevent unAble to call event
    //}

    //private void GameManager_OnPausedAction(object sender, EventArgs e)
    //{
    //    Show();
    //    OnPausedEvent?.Invoke(this, EventArgs.Empty);
    //}

    //private void GameManager_OnUnPausedAction(object sender, EventArgs e)
    //{
    //    Hide();
    //    OnUnPausedEvent?.Invoke(this, EventArgs.Empty);
    //}

    //private void Show()
    //{
    //    gameObject.SetActive(true);

    //    //resumeButton.Select();
    //}

    //private void Hide()
    //{
    //    gameObject.SetActive(false);
    //}
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WinningScreenUI : MonoBehaviour
{
    Button changeLevelButton;



    private void Start()
    {
        changeLevelButton = GetComponentInChildren<Button>(true);
        WinFlagGoal.instance.OnTriggerWinFlag += WinFlag_OnTriggerWin; 
    }

    void WinFlag_OnTriggerWin(object sender, System.EventArgs e)
    {
        
            changeLevelButton.gameObject.SetActive(true);
        
    }
}

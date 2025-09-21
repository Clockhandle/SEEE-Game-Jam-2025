using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Buttontype
{
    Switch1, Switch2
}
public class SwitchLevelButton : MonoBehaviour
{
    public  Buttontype buttonType;
    public Button[] page1Buttons;
    public Button[] page2Buttons;

    private Button thisButton;

    private void Start()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnSwitchClick);
    }

    private void OnSwitchClick()
    {
        if (buttonType == Buttontype.Switch1)
        {
            ShowPage1();
        }
        else if (buttonType == Buttontype.Switch2)
        {
            ShowPage2();
        }
    }

    private void ShowPage1()
    {
        foreach (var btn in page1Buttons)
            btn.gameObject.SetActive(true);

        foreach (var btn in page2Buttons)
            btn.gameObject.SetActive(false);
    }

    private void ShowPage2()
    {
        foreach (var btn in page1Buttons)
            btn.gameObject.SetActive(false);

        foreach (var btn in page2Buttons)
            btn.gameObject.SetActive(true);
    }
}
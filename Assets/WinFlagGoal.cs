using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WinFlagGoal : MonoBehaviour
{
    public static WinFlagGoal instance;

    public event EventHandler OnTriggerWinFlag;

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnTriggerWinFlag?.Invoke(this, EventArgs.Empty);
            WhiteFlash.instance.ActiveFlashScreen();
        }
    }
}

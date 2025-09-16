using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SteelDoor : MonoBehaviour
{
    public static event EventHandler OnSteelDoorUnlocked;


    private void Start()
    {
        UnlockDoorBomb.OnBombExplode += UnlockedBomb_OnBombActive;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null && player.HasKey())
            {
                OnSteelDoorUnlocked?.Invoke(this, EventArgs.Empty);
            }
        }
    }

 
    void UnlockedBomb_OnBombActive(object sender, EventArgs e)
    {
        Destroy(gameObject);  // create particle and shit
    }
}

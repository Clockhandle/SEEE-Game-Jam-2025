using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SteelDoor : MonoBehaviour
{
    public static event EventHandler OnSteelDoorUnlocked;

    public GameObject[] explodePrefabs;

    private void Start()
    {
        UnlockDoorBomb.OnBombExplode += UnlockedBomb_OnBombActive;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
          
            PlayerTest player = collision.gameObject.GetComponent<PlayerTest>();
            if (player != null && player.HasBomb())
            {
                OnSteelDoorUnlocked?.Invoke(this, EventArgs.Empty);
            }
        }
    }

 
    void UnlockedBomb_OnBombActive(object sender, EventArgs e)
    {
        CamShake.instance.ShakeExplosion();
        foreach(var obj in explodePrefabs)
        {
            GameObject explodePreb = Instantiate(obj, transform.position, Quaternion.identity);
            Destroy(explodePreb, 1f);
        }
        Destroy(gameObject);  // create particle and shit
    }
}

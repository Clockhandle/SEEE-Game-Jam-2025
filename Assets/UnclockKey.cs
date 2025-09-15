using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnclockKey : MonoBehaviour
{
    public event EventHandler OnGetUnlockkey;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnGetUnlockkey?.Invoke(this, EventArgs.Empty);

            Destroy(gameObject, .2f);
        }
    }
}

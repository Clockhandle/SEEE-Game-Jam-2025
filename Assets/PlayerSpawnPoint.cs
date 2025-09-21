using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private PlayerTest player;
    public float deathDuration;
    private Coroutine respawnCoroutine;
    
    private void Start()
    {
        DeathObj.OnDeath += DeathOobj_OnDeath;
        // Don't cache player reference here - find it when needed
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent MissingReferenceException
        DeathObj.OnDeath -= DeathOobj_OnDeath;
        
        // Stop any running coroutines to prevent accessing destroyed objects
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }
    }

    void DeathOobj_OnDeath(object sender, EventArgs e)
    {
        // Add null checks to prevent errors if this object is being destroyed
        if (this == null || gameObject == null) return;
        
        // Stop any existing respawn coroutine before starting a new one
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }
        
        respawnCoroutine = StartCoroutine(ReSpawn());
    }

    IEnumerator ReSpawn()
    {
        // Add null check at the start of coroutine
        if (this == null || gameObject == null) 
        {
            respawnCoroutine = null;
            yield break;
        }
        
        yield return new WaitForSeconds(deathDuration);

        // Additional null check after waiting - very important for scene transitions
        if (this == null || gameObject == null) 
        {
            respawnCoroutine = null;
            yield break;
        }

        // Find player fresh each time to avoid destroyed reference
        player = FindObjectOfType<PlayerTest>();
        
        if (player != null)
        {
            // Additional check to make sure we still exist before accessing transform
            if (this != null && gameObject != null)
            {
                player.transform.position = transform.position;

                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                Collider2D col = player.GetComponent<Collider2D>();

                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.gravityScale = 1f;
                }

                if (col != null)
                    col.enabled = true;

                // Reset death state
                player.SetDeath(false);
            }
        }
        else
        {
            Debug.LogWarning("PlayerSpawnPoint: Could not find PlayerTest to respawn!");
        }
        
        // Clear the coroutine reference
        respawnCoroutine = null;
    }
}

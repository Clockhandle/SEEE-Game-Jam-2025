using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockedDoorFlash : MonoBehaviour
{
    public static event EventHandler OnDoorUnlocked;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration;
    
    [Header("Actual Door Settings")]
    [SerializeField] private GameObject actualDoorObject; // Reference to the actual door GameObject
    [SerializeField] private bool manageDoorSprite = true; // Toggle to enable/disable sprite management
    
    [Header("Key Settings")]
    [SerializeField] private bool consumeKey = true; // Whether this door consumes a key when unlocked
    
    private bool doorUnlocked = false; // Prevent multiple unlocks

    private void Awake()
    {
        // Initially disable the actual door's sprite so it's hidden behind the locked door
        if (manageDoorSprite && actualDoorObject != null)
        {
            DisableActualDoorSprite();
        }
    }

    private void FlashEffect()
    {
        OnDoorUnlocked?.Invoke(this, EventArgs.Empty);
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i <= 3; i++)
        {
            spriteRenderer.material.SetInt("_Flash", 1);
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.material.SetInt("_Flash", 0);

            yield return new WaitForSeconds(flashDuration);
        }

        // Re-enable the actual door's sprite renderer to make it visible
        if (manageDoorSprite && actualDoorObject != null)
        {
            EnableActualDoorSprite();
        }
        
        // Destroy the locked door overlay
        Destroy(gameObject);
    }

    private void DisableActualDoorSprite()
    {
        // Disable the SpriteRenderer of the actual door to hide it initially
        if (actualDoorObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer doorRenderer))
        {
            doorRenderer.enabled = false;
        }
    }

    private void EnableActualDoorSprite()
    {
        // Re-enable the SpriteRenderer of the actual door to make it visible
        if (actualDoorObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer doorRenderer))
        {
            doorRenderer.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (doorUnlocked) return; // Prevent multiple unlocks
        
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerTest player = collision.gameObject.GetComponent<PlayerTest>();
            if (player != null && player.HasKey())
            {
                // Try to consume a key if this door requires it
                if (consumeKey)
                {
                    if (player.UseKey()) // This consumes one key
                    {
                        doorUnlocked = true;
                        FlashEffect();
                    }
                }
                else
                {
                    // Door doesn't consume keys (master key scenario)
                    doorUnlocked = true;
                    FlashEffect();
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SteelDoor : MonoBehaviour
{
    public GameObject[] explodePrefabs;
    private bool doorUnlocked = false; // Prevent multiple unlocks
    
    [Header("Actual Door Settings")]
    [SerializeField] private GameObject actualDoorObject; // Reference to the actual door GameObject
    [SerializeField] private bool manageDoorSprite = true; // Toggle to enable/disable sprite management

    private void Start()
    {
        // Initially disable the actual door's sprite so it's hidden behind the steel door
        if (manageDoorSprite && actualDoorObject != null)
        {
            DisableActualDoorSprite();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (doorUnlocked) return; // Prevent multiple unlocks
        
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerTest player = collision.gameObject.GetComponent<PlayerTest>();
            if (player != null && player.HasBomb())
            {
                // Find ANY collected bomb (simplest approach)
                UnlockDoorBomb anyBomb = FindAnyCollectedBomb();
                
                if (anyBomb != null)
                {
                    // Consume the bomb from player's inventory
                    if (player.UseBomb())
                    {
                        doorUnlocked = true;
                        Debug.Log($"Door {name} unlocked! Using bomb: {anyBomb.name}");
                        
                        // Attach this bomb to THIS door and make it explode
                        AttachBombToDoor(anyBomb);
                    }
                }
                else
                {
                    Debug.Log($"No collected bomb found for door {name}");
                }
            }
        }
    }

    private UnlockDoorBomb FindAnyCollectedBomb()
    {
        // Find all bombs in the scene
        UnlockDoorBomb[] allBombs = FindObjectsOfType<UnlockDoorBomb>();
        
        foreach (UnlockDoorBomb bomb in allBombs)
        {
            // Return the first bomb that is collected but not consumed
            if (bomb.IsBombCollected && !bomb.IsBombConsumed)
            {
                return bomb;
            }
        }
        
        return null;
    }

    private void AttachBombToDoor(UnlockDoorBomb bomb)
    {
        // Force this bomb to attach to THIS door
        bomb.AttachToSpecificDoor(this);
    }

    private void DisableActualDoorSprite()
    {
        // Disable the SpriteRenderer of the actual door to hide it initially
        if (actualDoorObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer doorRenderer))
        {
            doorRenderer.enabled = false;
            Debug.Log($"Disabled sprite renderer for door: {actualDoorObject.name}");
        }
    }

    private void EnableActualDoorSprite()
    {
        // Re-enable the SpriteRenderer of the actual door to make it visible
        if (actualDoorObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer doorRenderer))
        {
            doorRenderer.enabled = true;
            Debug.Log($"Enabled sprite renderer for door: {actualDoorObject.name}");
        }
    }

    public void ExplodeDoor()
    {
        Debug.Log($"Door {name} exploding!");
        CamShake.instance.ShakeExplosion();
        
        // Enable the actual door's sprite renderer before destroying the steel door
        if (manageDoorSprite && actualDoorObject != null)
        {
            EnableActualDoorSprite();
        }
        
        foreach(var obj in explodePrefabs)
        {
            GameObject explodePreb = Instantiate(obj, transform.position, Quaternion.identity);
            Destroy(explodePreb, 1f);
        }
        
        Destroy(gameObject);
    }
}
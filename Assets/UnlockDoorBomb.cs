using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoorBomb : MonoBehaviour
{
    private Transform player;

    public static event EventHandler OnGetUnlockBomb;
    public static event EventHandler OnBombExplode;

    public Transform followPoint;
    private bool canfollowPlayer = false;
    private bool bombCollected = false; // Prevent multiple collections
    private bool bombConsumed = false; // Prevent multiple consumptions
    private bool isAttachedToDoor = false;

    public float followSpeed = 5f;  // how fast it chases target
    public float smoothDamp = 0.3f;

    private Vector3 velocity;
    private SteelDoor attachedDoor; // The door this bomb is currently attached to

    [Header("Flash")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.2f;

    // Public properties to check bomb state
    public bool IsBombCollected => bombCollected;
    public bool IsBombConsumed => bombConsumed;

    private void OnEnable()
    {
        // Find player fresh - don't cache across scene transitions
        RefreshPlayerReference();
    }

    private void RefreshPlayerReference()
    {
        PlayerTest playerTest = FindObjectOfType<PlayerTest>();
        if (playerTest != null)
        {
            player = playerTest.GetComponent<Transform>();
        }
        else
        {
            player = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (bombCollected) return; // Prevent multiple collections
        
        if (collision.CompareTag("Player"))
        {
            if (player == null)
            {
                player = collision.transform;
            }
            
            bombCollected = true;
            canfollowPlayer = true;

            OnGetUnlockBomb?.Invoke(this, EventArgs.Empty);
            Debug.Log($"Bomb {name} collected by player!");
        }
    }

    // Simple method: Attach this bomb to a specific door
    public void AttachToSpecificDoor(SteelDoor door)
    {
        if (bombConsumed) return; // Already consumed
        
        attachedDoor = door;
        isAttachedToDoor = true;
        bombConsumed = true;
        
        Debug.Log($"Bomb {name} attaching to door: {door.name}");
        StartCoroutine(FlashRoutine());
    }

    void LateUpdate()
    {
        if (isAttachedToDoor && attachedDoor != null)
        {
            // Stick to the attached door
            transform.position = attachedDoor.transform.position;
            return;
        }

        // Refresh player reference if it's null (after scene transition)
        if (player == null && canfollowPlayer)
        {
            RefreshPlayerReference();
        }

        if (canfollowPlayer && player != null && !bombConsumed)
        {
            // target follow position (slightly behind player)
            Vector3 targetPos = followPoint.position;

            // springy follow movement
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref velocity,
                smoothDamp,
                followSpeed
            );

            transform.Rotate(0f, 0f, 180f * Time.deltaTime);
        }
    }

    private IEnumerator FlashRoutine()
    {
        Debug.Log($"Bomb {name} starting flash routine...");
        
        for (int i = 0; i <= 4; i++)
        {
            spriteRenderer.material.SetInt("_Flash", 1);
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.material.SetInt("_Flash", 0);
            yield return new WaitForSeconds(flashDuration);
        }
        
        Debug.Log($"Bomb {name} flash routine complete, exploding door...");
        
        // Explode the attached door
        if (attachedDoor != null)
        {
            attachedDoor.ExplodeDoor();
        }
        
        OnBombExplode?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }
}

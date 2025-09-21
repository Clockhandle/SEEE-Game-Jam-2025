using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnclockKey : MonoBehaviour
{
    private Transform player;

    public static event EventHandler OnGetUnlockkey;

    public Transform followPoint;
    private bool canfollowPlayer = false;
    private bool keyCollected = false; // Prevent multiple collections
    private bool keyConsumed = false; // Prevent multiple consumptions

    public float followSpeed = 5f;
    public float smoothDamp = 0.3f;

    private Vector3 velocity;

    private void OnEnable()
    {
        // Find player fresh - don't cache across scene transitions
        RefreshPlayerReference();
        
        // Listen for key consumption events
        PlayerTest.OnKeyUsed += HandleKeyUsed;
    }
    
    private void OnDisable()
    {
        PlayerTest.OnKeyUsed -= HandleKeyUsed;
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
        if (keyCollected) return; // Prevent multiple collections
        
        if (collision.CompareTag("Player"))
        {
            if(player == null)
            {
                player = collision.transform;
            }
            
            keyCollected = true;
            canfollowPlayer = true;
            
            OnGetUnlockkey?.Invoke(this, EventArgs.Empty);
            
            // DON'T destroy - let it follow the player
            // The key will be destroyed when a door consumes it
        }
    }

    private void HandleKeyUsed(object sender, EventArgs e)
    {
        // Only destroy if this key was collected and hasn't been consumed yet
        if (keyCollected && !keyConsumed)
        {
            keyConsumed = true;
            ConsumeKey();
        }
    }

    public void ConsumeKey()
    {
        StartCoroutine(ConsumeEffect());
    }
    
    private IEnumerator ConsumeEffect()
    {
        // Optional: Add destruction effect
        float duration = 0.3f;
        Vector3 startScale = transform.localScale;
        
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float progress = t / duration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, progress);
            yield return null;
        }
        
        Destroy(gameObject);
    }

    void LateUpdate()
    {
        // Refresh player reference if it's null (after scene transition)
        if (player == null && canfollowPlayer)
        {
            RefreshPlayerReference();
        }

        if(canfollowPlayer && player != null && !keyConsumed)
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
}

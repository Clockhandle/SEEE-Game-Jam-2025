using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketJumpExplosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float maxExplosionForce = 800f;
    [SerializeField] private float minExplosionForce = 100f;
    [SerializeField] private LayerMask explosionTargets = -1;
    [SerializeField] private float lifetime = 2f;

    private Transform playerTransform;
    private float distanceToWall;
    private float maxDistance;
    private bool isPlayerMoving;
    private bool useMovementBasedLogic;

    public void Initialize(Transform player, float wallDistance, float maxWallDistance, bool playerMoving, bool movementBased)
    {
        playerTransform = player;
        distanceToWall = wallDistance;
        maxDistance = maxWallDistance;
        isPlayerMoving = playerMoving;
        useMovementBasedLogic = movementBased;
        
        // Trigger explosion immediately
        Explode();
    }

    void Start()
    {
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    private void Explode()
    {
        // Find all objects in explosion radius
        Collider2D[] affectedObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius, explosionTargets);

        foreach (Collider2D obj in affectedObjects)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                ApplyExplosionForce(rb, obj.transform);
            }
        }

        Debug.Log($"Explosion triggered! Affected {affectedObjects.Length} objects");
    }

    private void ApplyExplosionForce(Rigidbody2D rb, Transform target)
    {
        // Calculate force based on distance to wall
        float normalizedDistance = Mathf.Clamp01(distanceToWall / maxDistance);
        float explosionForce = Mathf.Lerp(maxExplosionForce, minExplosionForce, normalizedDistance);

        Vector2 forceDirection;

        // Check if this is the player and use movement-based logic
        if (target == playerTransform && useMovementBasedLogic)
        {
            if (isPlayerMoving)
            {
                // Player is moving - push upward only
                forceDirection = Vector2.up;
                Debug.Log("Player moving - explosion pushing UP only");
            }
            else
            {
                // Player not moving - push away from explosion
                forceDirection = (target.position - transform.position).normalized;
                Debug.Log("Player stationary - explosion pushing AWAY");
            }
        }
        else
        {
            // Standard explosion logic for other objects
            forceDirection = (target.position - transform.position).normalized;
        }

        // Apply the force
        rb.AddForce(forceDirection * explosionForce, ForceMode2D.Impulse);
        
        Debug.Log($"Applied explosion force: {explosionForce} in direction: {forceDirection} to {target.name}");
    }

    // Visualize explosion radius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTest : MonoBehaviour
{
    [Header("Bomb Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private LayerMask explosionTargets = -1; // What gets affected by explosion
    [SerializeField] private GameObject explosionEffect; // Optional visual effect

    private bool hasExploded = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if bomb hit a wall (based on your collision matrix)
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !hasExploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Create explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Find all objects in explosion radius
        Collider2D[] affectedObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius, explosionTargets);

        foreach (Collider2D obj in affectedObjects)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Calculate direction and distance from explosion
                Vector2 direction = (obj.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(transform.position, obj.transform.position);

                // Calculate force based on distance (closer = more force)
                float forceMagnitude = explosionForce * (1f - (distance / explosionRadius));

                // Apply force
                rb.AddForce(direction * forceMagnitude, ForceMode2D.Impulse);
            }
        }

        // Destroy the bomb
        Destroy(gameObject);
    }

    // Replace Gizmos.DrawWireCircle with Gizmos.DrawWireSphere in OnDrawGizmosSelected
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

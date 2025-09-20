using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeathObj : MonoBehaviour
{
    public static event EventHandler OnDeath;

    [SerializeField] private float jumpForce = 5f; // tweak in Inspector
    [SerializeField] private float fallGravity = 2f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Active Cam Shake
            CamShake.instance.ShakeDeath();

            PlayerTest player = collision.gameObject.GetComponent<PlayerTest>();
            player.SetDeath(true);

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            Collider2D playerCollider = collision.gameObject.GetComponent<Collider2D>();

            if (rb != null)
            {
      
                rb.velocity = Vector2.zero;
             // add a bit force before falling
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                rb.gravityScale = fallGravity;
            }

             playerCollider.enabled = false;

            OnDeath?.Invoke(this, EventArgs.Empty);

           // GameManager.Instance.ReLoadScene();


        }
    }
}


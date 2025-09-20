using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float impactRange;
    public float force;

    public LayerMask impactLayer;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || 
            collision.gameObject.layer == LayerMask.NameToLayer("Destroyable") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Wall") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Bomb") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            // STOP THE BOMB MOVEMENT
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;      // Stop all movement
                rb.angularVelocity = 0f;         // Stop rotation
                rb.isKinematic = true;           // Make it immovable
            }
            
            Explode();
            Destroy(gameObject, .3f);
        }
    }

    private void Explode()
    {
        anim.SetTrigger("Explode");
        //Explode sound
        SoundFXManage.Instance.PlayExplodeSound();
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, impactRange, impactLayer);
        foreach(Collider2D obj in objects)
        {
            Vector2 direction = (obj.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, obj.transform.position);

            float distanceFactor = 1 - (distance / impactRange);
            distanceFactor = Mathf.Clamp(distanceFactor, 0.2f, 0.8f);

            obj.GetComponent<Rigidbody2D>().AddForce(direction * force * distanceFactor, ForceMode2D.Impulse);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, impactRange);
    }
}

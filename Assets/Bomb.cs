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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, impactRange, impactLayer);
        foreach(Collider2D obj in objects)
        {
            Vector2 direction = (obj.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, obj.transform.position);

            float distanceFactor = 1 - (distance / impactRange);
            distanceFactor = Mathf.Clamp(distanceFactor, 0.2f, 0.8f);

            obj.GetComponent<Rigidbody2D>().AddForce(direction * force * distanceFactor, ForceMode2D.Impulse);
        }
        anim.SetTrigger("Explode");

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, impactRange);
    }
}

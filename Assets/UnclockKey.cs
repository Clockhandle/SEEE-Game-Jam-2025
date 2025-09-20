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

    public float followSpeed = 5f;  // how fast it chases target
    public float smoothDamp = 0.3f;

    private Vector3 velocity;

    private void OnEnable()
    {
        player = FindObjectOfType<PlayerTest>().GetComponent<Transform>();

        UnlockedDoorFlash.OnDoorUnlocked += HandleDoorUnlocked;
    }
    private void OnDisable()
    {
        UnlockedDoorFlash.OnDoorUnlocked -= HandleDoorUnlocked;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(player == null)
            {
                player = collision.transform;
               
            }
            canfollowPlayer = true;
            
            OnGetUnlockkey?.Invoke(this, EventArgs.Empty);
            //Destroy(gameObject, .2f);

        }
    }

    void LateUpdate()
    {
        if(canfollowPlayer && player != null)
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

    private void HandleDoorUnlocked(object sender, EventArgs e)
    {
        Destroy(gameObject); 
    }
}

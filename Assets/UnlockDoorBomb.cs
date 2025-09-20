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
    private bool isAttachedToDoor = false;

    public float followSpeed = 5f;  // how fast it chases target
    public float smoothDamp = 0.3f;

    private Vector3 velocity;
    private Transform doorTarget;

    [Header("Flash")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration;

    private void OnEnable()
    {
        player = FindObjectOfType<PlayerTest>().GetComponent<Transform>();
        doorTarget = FindObjectOfType<SteelDoor>().transform;   
       SteelDoor.OnSteelDoorUnlocked += HandleDoorUnlocked;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player == null)
            {
                player = collision.transform;

            }
            canfollowPlayer = true;

            OnGetUnlockBomb?.Invoke(this, EventArgs.Empty);
            //Destroy(gameObject, .2f);

        }
    }

    void LateUpdate()
    {

        if (isAttachedToDoor)
        {
            // Stick to door
            transform.position = doorTarget.position;
            return;
        }

        if (canfollowPlayer && player != null)
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
        isAttachedToDoor = true;
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i <= 4; i++)
        {
            spriteRenderer.material.SetInt("_Flash", 1);
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.material.SetInt("_Flash", 0);

            yield return new WaitForSeconds(flashDuration);
        }
        OnBombExplode?.Invoke(this, EventArgs.Empty );  
        Destroy(gameObject);

    }
}

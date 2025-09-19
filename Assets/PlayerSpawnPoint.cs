using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private PlayerTest player;
    public float deathDuration;
    private void Start()
    {
        DeathObj.OnDeath += DeathOobj_OnDeath;
        player = FindObjectOfType<PlayerTest>();
    }

    void DeathOobj_OnDeath(object sender, EventArgs e)
    {
        StartCoroutine(ReSpawn());
    }

    IEnumerator ReSpawn()
    {
        yield return new WaitForSeconds(deathDuration);

        if (player != null)
        {

            player.transform.position = transform.position;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            Collider2D col = player.GetComponent<Collider2D>();

            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = 1f;
            }

            if (col != null)
                col.enabled = true;

            // Reset death state
            player.SetDeath(false);
        }
    }
}

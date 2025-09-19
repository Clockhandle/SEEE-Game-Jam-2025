using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockedDoorFlash : MonoBehaviour
{
    public static event EventHandler OnDoorUnlocked;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration;




    private void Awake()
    {
    
      
    }

  
    private void FlashEffect()
    {
        OnDoorUnlocked?.Invoke(this, EventArgs.Empty);
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for(int i = 0; i<= 3; i++)
        {
            spriteRenderer.material.SetInt("_Flash", 1);
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.material.SetInt("_Flash", 0);

            yield return new WaitForSeconds(flashDuration);
        }
        Destroy(gameObject);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();    
            if(player!= null && player.HasKey())
            {
                FlashEffect();
            }
        }
    }
}

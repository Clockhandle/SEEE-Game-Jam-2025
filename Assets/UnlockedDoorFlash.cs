using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockedDoorFlash : MonoBehaviour
{
    public static event EventHandler OnDoorUnlocked;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration;
    
    [Header("Open Door Settings")]
    [SerializeField] private GameObject openDoorObject; // Reference to existing door GameObject in scene
    [SerializeField] private bool activateOpenDoor = true; // Toggle to enable/disable activation

    private void Awake()
    {
        // Ensure the open door is initially disabled
        if (openDoorObject != null)
        {
            openDoorObject.SetActive(false);
        }
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
        
        // Activate open door before destroying the locked door
        if (activateOpenDoor && openDoorObject != null)
        {
            ActivateOpenDoor();
        }
        
        Destroy(gameObject);
    }

    private void ActivateOpenDoor()
    {
        // Simply activate the existing door GameObject
        openDoorObject.SetActive(true);
        
        // Ensure the door renderer has full alpha immediately
        if (openDoorObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer openDoorRenderer))
        {
            Color doorColor = openDoorRenderer.color;
            doorColor.a = 1f; // Set alpha to full opacity
            openDoorRenderer.color = doorColor;
        }
    }
    
    // Optional: Smooth fade-in effect for the open door
    private IEnumerator FadeInOpenDoor(SpriteRenderer doorRenderer)
    {
        Color originalColor = doorRenderer.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        
        doorRenderer.color = transparentColor;
        
        float fadeSpeed = 2f; // Adjust fade speed as needed
        float elapsedTime = 0f;
        
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * fadeSpeed;
            doorRenderer.color = Color.Lerp(transparentColor, originalColor, elapsedTime);
            yield return null;
        }
        
        doorRenderer.color = originalColor;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerTest player = collision.gameObject.GetComponent<PlayerTest>();    
            if(player != null && player.HasKey())
            {
                FlashEffect();
            }
        }
    }
}

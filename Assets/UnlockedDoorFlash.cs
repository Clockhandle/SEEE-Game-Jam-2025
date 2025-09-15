using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockedDoorFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration;

    private UnclockKey key;

    private void Awake()
    {
        key = FindObjectOfType<UnclockKey>();   
    }

    private void Start()
    {
        key.OnGetUnlockkey += FlashEffect;
    }
    public void FlashEffect(object sender, System.EventArgs e)
    {
        StartCoroutine(FlashRoutine());
    }

    public IEnumerator FlashRoutine()
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
}

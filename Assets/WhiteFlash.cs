using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteFlash : MonoBehaviour
{
   public static WhiteFlash instance;

    private CanvasGroup canvasGroup;
    private bool flash = false;
    public float flashDurationMultipler;

    private void Awake()
    {
        instance = this;    
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (flash)
        {
            canvasGroup.alpha = canvasGroup.alpha - Time.deltaTime* flashDurationMultipler;
            if(canvasGroup.alpha < 0)
            {
                canvasGroup.alpha = 0;
                flash = false;
            }
        }
    }

    public void ActiveFlashScreen()
    {
        flash = true;
        canvasGroup.alpha = 1;  
    }
}

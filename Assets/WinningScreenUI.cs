using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WinningScreenUI : MonoBehaviour
{
    Button changeLevelButton;

    [Header("Lens Distortion UI Fix")]
    [SerializeField] private bool fixLensDistortionInput = true;
    [SerializeField] private Canvas uiCanvas;

    private void Start()
    {
        changeLevelButton = GetComponentInChildren<Button>(true);
        WinFlagGoal.instance.OnTriggerWinFlag += WinFlag_OnTriggerWin;
        
        if (fixLensDistortionInput)
        {
            SetupLensDistortionInputFix();
        }
    }

    private void SetupLensDistortionInputFix()
    {
        // Ensure canvas is in Screen Space Camera mode (to get visual distortion)
        if (uiCanvas == null)
            uiCanvas = GetComponentInParent<Canvas>();
            
        if (uiCanvas != null && uiCanvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            uiCanvas.worldCamera = Camera.main;
            Debug.Log("Canvas set to Screen Space Camera for lens distortion visual effect");
        }

        // Add the input compensation system
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            // Remove default input module
            StandaloneInputModule defaultInput = eventSystem.GetComponent<StandaloneInputModule>();
            if (defaultInput != null)
                defaultInput.enabled = false;

            // Add our lens distortion compensated input module
            if (eventSystem.GetComponent<LensDistortionInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<LensDistortionInputModule>();
                Debug.Log("Added lens distortion input compensation");
            }
        }
    }

    void WinFlag_OnTriggerWin(object sender, System.EventArgs e)
    {
        changeLevelButton.gameObject.SetActive(true);
    }
}
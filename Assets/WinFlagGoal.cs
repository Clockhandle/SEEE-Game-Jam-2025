using System;
using UnityEngine;

public class WinFlagGoal : MonoBehaviour
{
    public static WinFlagGoal instance;

    public event EventHandler OnTriggerWinFlag;
    
    [Header("Win Flag Settings")]
    [SerializeField] private bool allowMultipleTriggers = false; // For debugging purposes
    
    private bool hasBeenTriggered = false; // Prevent multiple triggers

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Prevent multiple triggers unless explicitly allowed
        if (hasBeenTriggered && !allowMultipleTriggers)
        {
            Debug.Log("WinFlag already triggered, ignoring subsequent triggers");
            return;
        }
        
        if (collision.CompareTag("Player"))
        {
            hasBeenTriggered = true;
            Debug.Log("WinFlag triggered! Level completed.");
            
            OnTriggerWinFlag?.Invoke(this, EventArgs.Empty);
            WhiteFlash.instance.ActiveFlashScreen();
        }
    }
    
    // Optional: Method to reset the flag (useful for testing or special cases)
    public void ResetWinFlag()
    {
        hasBeenTriggered = false;
        Debug.Log("WinFlag reset - can be triggered again");
    }
    
    // Optional: Check if flag has been triggered
    public bool HasBeenTriggered => hasBeenTriggered;
}
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform playerTransform;
    
    [Header("Mouse Flipping Settings")]
    [SerializeField] private bool useMouseFlipping = true;
    [SerializeField] private bool compensateLensDistortion = true;
    
    private PlayerInput playerInput;
    private InputAction moveAction;
    private Camera mainCamera;
    private string IdleState = "Player_Idle";
    private string WalkState = "Player_Walking";

    void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        
        // Auto-find player if not assigned
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
        
        playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null)
        {
            OnInitialization();
        }
    }

    void Update()
    {
        HandleMovementAnimation();
        
        if (useMouseFlipping)
        {
            HandleMouseFlipping();
        }
    }

    private void OnInitialization()
    {
        moveAction = playerInput.actions["Movement"];
    }

    private void HandleMovementAnimation()
    {
        if (moveAction == null) return;

        float moveInput = moveAction.ReadValue<float>();
        
        // Handle animation based on movement input
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(WalkState))
            {
                animator.Play(WalkState);
            }
        }
        else
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(IdleState))
            {
                animator.Play(IdleState);
            }
        }
    }

    private void HandleMouseFlipping()
    {
        if (mainCamera == null || playerTransform == null) return;

        Vector3 mouseScreenPos = Input.mousePosition;
        
        // Apply lens distortion compensation if enabled
        if (compensateLensDistortion)
        {
            mouseScreenPos = CompensateLensDistortion(mouseScreenPos);
        }
        
        // Convert mouse position to world space
        mouseScreenPos.z = mainCamera.nearClipPlane;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        
        // Calculate direction from player to mouse
        Vector2 mouseDirection = (mouseWorldPos - playerTransform.position);
        
        // Flip based on horizontal direction
        if (mouseDirection.x < 0) // Mouse is to the left of player
        {
            spriteRenderer.flipX = true;
        }
        else if (mouseDirection.x > 0) // Mouse is to the right of player
        {
            spriteRenderer.flipX = false;
        }
        // Don't flip if mouse is directly above/below (mouseDirection.x ≈ 0)
    }

    private Vector3 CompensateLensDistortion(Vector3 screenPos)
    {
        // Get lens distortion from volume stack (same as in GunShoot)
        var volumeStack = UnityEngine.Rendering.VolumeManager.instance.stack;
        UnityEngine.Rendering.Universal.LensDistortion lensDistortion = volumeStack.GetComponent<UnityEngine.Rendering.Universal.LensDistortion>();

        if (lensDistortion != null && lensDistortion.active)
        {
            // Convert screen position to normalized coordinates (0-1)
            Vector2 normalizedPos = new Vector2(
                screenPos.x / Screen.width,
                screenPos.y / Screen.height
            );

            // Apply inverse distortion correction
            Vector2 correctedPos = UndistortPosition(normalizedPos, lensDistortion.intensity.value);

            // Convert back to screen coordinates
            screenPos.x = correctedPos.x * Screen.width;
            screenPos.y = correctedPos.y * Screen.height;
        }

        return screenPos;
    }

    private Vector2 UndistortPosition(Vector2 normalizedPos, float distortionIntensity)
    {
        // Convert to centered coordinates (-0.5 to 0.5)
        Vector2 centered = normalizedPos - Vector2.one * 0.5f;
        
        // Calculate distance from center
        float distance = centered.magnitude;
        
        // Apply inverse barrel distortion
        float distortedDistance = distance;
        if (distance > 0.001f) // Avoid division by zero
        {
            // Inverse of the barrel distortion formula
            distortedDistance = distance / (1.0f + distortionIntensity * distance * distance);
        }
        
        // Scale the centered position
        Vector2 corrected = centered.normalized * distortedDistance;
        
        // Convert back to normalized coordinates
        return corrected + Vector2.one * 0.5f;
    }

    private void OnDestroy()
    {
        // No event subscriptions to clean up in this version
    }

    // Optional: Public method to toggle mouse flipping at runtime
    public void SetMouseFlipping(bool enabled)
    {
        useMouseFlipping = enabled;
    }

    // Optional: Public method to toggle lens distortion compensation
    public void SetLensDistortionCompensation(bool enabled)
    {
        compensateLensDistortion = enabled;
    }
}

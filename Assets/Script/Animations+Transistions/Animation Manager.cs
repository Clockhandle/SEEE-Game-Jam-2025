using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationManager : MonoBehaviour
{
    public static PlayerAnimationManager Instance;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;
    private string IdleState = "Idle";
    private string WalkState = "Walking";

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null)
        {
            OnInitialization();
        }
    }

    void Update()
    {
    }
    private void OnInitialization()
    {
        playerInput.actions["Movement"].performed += OnMovementPerformed;
        playerInput.actions["Movement"].canceled += OnMovementCanceled;
    }

    private void OnDestroy()
    {
        playerInput.actions["Movement"].performed -= OnMovementPerformed;
        playerInput.actions["Movement"].canceled -= OnMovementCanceled;
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        var keyName = context.control.name.ToLower();
        if (keyName == "a")
        {
            spriteRenderer.flipX = true;
        }
        else if (keyName == "d")
        {
            spriteRenderer.flipX = false;
        }
        animator.Play(WalkState);
    }
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        animator.Play(IdleState);
    }
}

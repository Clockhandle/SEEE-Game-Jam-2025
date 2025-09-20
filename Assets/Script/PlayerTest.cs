using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTest : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float friction = 10f; // Friction when no input during normal movement

    [Header("Rocket Jump Settings")]
    [SerializeField] private float rocketJumpCooldown = 0.5f; // Time to preserve rocket jump momentum
    [SerializeField] private float maxHorizontalSpeed = 15f; // Maximum horizontal speed during rocket jump

    [Header("Wall Sliding Settings")]
    [SerializeField] private bool enableWallSliding = true; // Enable wall sliding when airborne

    [Header("Winning")]
    bool isWinning = false;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private float moveInput;
    private float lastRocketJumpTime = -10f; // Track when last rocket jump occurred

    // Simplified wall collision tracking
    private bool isGrounded = false;
    private bool isTouchingWall = false;

    // Public property to check if player is giving movement input
    public bool IsMoving => Mathf.Abs(moveInput) > 0.1f;

    // Property to check if we're in rocket jump cooldown period
    public bool IsInRocketJumpCooldown => Time.time - lastRocketJumpTime < rocketJumpCooldown;


    [Header("Death")]
    bool isDead;
    public GameObject deathEffect;

    [Header("UnlockEvent")]
    private int keyCount = 0; // Changed from bool hasKey to int keyCount
    private int bombCount = 0; // Changed from bool hasBomb to int bombCount

    // Add event for key consumption
    public static event EventHandler OnKeyUsed;
    // Add event for bomb consumption
    public static event EventHandler OnBombUsed;


    void Awake()
    {
        isWinning = false;
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Movement"];
    }
    private void Start()
    {
        DeathObj.OnDeath += DeathObj_OnPlayerDeath;
        UnclockKey.OnGetUnlockkey += Key_OnGetUnlockedKey;
        UnlockDoorBomb.OnGetUnlockBomb += Bomb_OnGetBomb;
        WinFlagGoal.instance.OnTriggerWinFlag += WinFlag_OnWinning;
    }

    void OnEnable() => moveAction.Enable();
    void OnDisable() => moveAction.Disable();

    void WinFlag_OnWinning(object sender, EventArgs e)
    {
        isWinning = true;
    }
    void Update()
    {
        if (isDead) return;
        if (isWinning) return;

        moveInput = moveAction.ReadValue<float>();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (isWinning) return;

        if (IsInRocketJumpCooldown)
        {
            // During rocket jump cooldown, blend movement input with existing momentum
            if (Mathf.Abs(moveInput) > 0.1f)
            {
                // Add limited movement influence to existing velocity instead of overriding
                float movementInfluence = moveInput * moveSpeed * 0.3f;
                float newXVelocity = rb.velocity.x + movementInfluence * Time.fixedDeltaTime * 10f;
                
                // Clamp horizontal velocity to maximum speed limit
                newXVelocity = Mathf.Clamp(newXVelocity, -maxHorizontalSpeed, maxHorizontalSpeed);
                
                Vector2 newVelocity = new Vector2(newXVelocity, rb.velocity.y);
                rb.velocity = newVelocity;
            }
            else
            {
                // No input but still apply speed limit to prevent excessive velocities
                float clampedXVelocity = Mathf.Clamp(rb.velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed);
                if (Mathf.Abs(clampedXVelocity - rb.velocity.x) > 0.01f)
                {
                    rb.velocity = new Vector2(clampedXVelocity, rb.velocity.y);
                }
            }
        }
        else
        {
            // Handle wall sliding: Zero out horizontal velocity when touching wall and airborne
            if (enableWallSliding && isTouchingWall && !isGrounded)
            {
                // Player is airborne and touching a wall - zero horizontal velocity to prevent sticking
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
            else if (Mathf.Abs(moveInput) > 0.1f)
            {
                // Normal movement when not wall sliding
                Vector2 normalMovement = new Vector2(moveInput * moveSpeed, rb.velocity.y);
                rb.velocity = normalMovement;
            }
            else
            {
                // No input - apply friction to stop horizontal movement
                float currentXVelocity = rb.velocity.x;
                float frictionForce = friction * Time.fixedDeltaTime;
                
                // Apply friction towards zero velocity
                if (Mathf.Abs(currentXVelocity) > frictionForce)
                {
                    // Reduce velocity by friction amount in the opposite direction
                    float newXVelocity = currentXVelocity - Mathf.Sign(currentXVelocity) * frictionForce;
                    rb.velocity = new Vector2(newXVelocity, rb.velocity.y);
                }
                else
                {
                    // Stop completely if velocity is very small
                    rb.velocity = new Vector2(0f, rb.velocity.y);
                }
            }
        }
    }

    // Simplified collision detection
    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollision(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckCollision(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            // Reset collision states when leaving wall
            isTouchingWall = false;
            isGrounded = false;
        }
    }

    private void CheckCollision(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            // Simple approach: if touching any wall, mark as touching wall
            isTouchingWall = true;
            
            // Check if any contact point indicates ground collision (normal pointing up)
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.7f) // Ground collision (normal pointing up)
                {
                    isGrounded = true;
                    break;
                }
            }
        }
    }

    // Method for GunShoot to notify about rocket jump
    public void OnRocketJump()
    {
        lastRocketJumpTime = Time.time;
    }

    //Death
    void DeathObj_OnPlayerDeath(object sender, EventArgs e)
    {
        GameObject deathEffect = Instantiate(this.deathEffect, transform.position, Quaternion.identity);
        Destroy(deathEffect, 1f);
    }

    public void SetDeath(bool value)
    {
        isDead = value;
    }
    void Key_OnGetUnlockedKey(object sender, EventArgs e)
    {
        keyCount++; // Increment key count instead of setting to true
        Debug.Log($"Key collected! Total keys: {keyCount}");
    }

    void Bomb_OnGetBomb(object sender, EventArgs e)
    {
        bombCount++; // Instead of hasBomb = true;
        Debug.Log($"Bomb collected! Total bombs: {bombCount}");
    }

    public bool HasKey() => keyCount > 0; // Returns true if player has at least one key

    // Update the UseKey method to fire an event
    public bool UseKey()
    {
        if (keyCount > 0)
        {
            keyCount--;
            Debug.Log($"Key used! Remaining keys: {keyCount}");
            
            // Fire event to notify keys that one was consumed
            OnKeyUsed?.Invoke(this, EventArgs.Empty);
            return true;
        }
        return false;
    }

    // Optional: Get current key count
    public int GetKeyCount() => keyCount;

    public bool HasBomb() => bombCount > 0;

    // Add a UseBomb method similar to UseKey
    public bool UseBomb()
    {
        if (bombCount > 0)
        {
            bombCount--;
            Debug.Log($"Bomb used! Remaining bombs: {bombCount}");
            
            // Fire event to notify bombs that one was consumed
            OnBombUsed?.Invoke(this, EventArgs.Empty);
            return true;
        }
        return false;
    }



    // Debug visualization
    void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position;
        
        // Ground indicator
        Gizmos.color = isGrounded ? Color.green : Color.gray;
        Gizmos.DrawWireCube(center + Vector3.down * 0.8f, Vector3.one * 0.2f);
        
        // Wall indicator
        Gizmos.color = isTouchingWall ? Color.red : Color.gray;
        Gizmos.DrawWireSphere(center, 0.3f);
        
        // Wall sliding active indicator
        if (enableWallSliding && isTouchingWall && !isGrounded)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(center + Vector3.up * 0.8f, Vector3.one * 0.15f);
        }
    }
}
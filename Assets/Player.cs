using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Animator anim;

    public float walkSpeed;
    [SerializeField] private float airControlForce = 5f;
    public float rotateSpeed;
    private float moveInput;
    public bool isGround;
    private Rigidbody2D rb;
    public LayerMask groundLayer;
    float checkSize = 0.8f;


    public bool canJump = true;

    public float jumpValue = 0;

    [Header("JumpBar")]
    public Image fillBar;
    public Image backGroundBar;

    [Header("Gravity Region")]
    [SerializeField] private float gravityScale;
    [SerializeField] float fallGravityMult;
    [SerializeField] float maxFallSpeed;


    [Header("ExplosionRange")]

    [SerializeField] float startRadius;
    [SerializeField] float maxRadius;
    private float explandSpeed;
    public LayerMask destroyObjLayer;

    private float currentExplodeRaidus;

    [Header("KeyEvent")]
    UnclockKey key;
    UnlockDoorBomb unlockBomb;
    private bool hasKey = false;
    private bool hasBomb = false;

    [Header("Death")]
    bool isDead;
    public GameObject deathEffect;

    [Header("Winning")]
    bool isWinning = false;

    private void Awake()
    {
        isWinning = false;
        anim = GetComponentInChildren<Animator>();

       
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        key = FindObjectOfType<UnclockKey>();

        unlockBomb = FindObjectOfType<UnlockDoorBomb>();

        currentExplodeRaidus = startRadius;
      
        if(key!= null)
            key.OnGetUnlockkey += Key_OnGetUnlockedKey;
        if(unlockBomb!= null)
            unlockBomb.OnGetUnlockBomb += Bomb_OnGetBomb;

        DeathObj.OnDeath += DeathObj_OnPlayerDeath;
        WinFlagGoal.instance.OnTriggerWinFlag += WinFlag_OnWinning;


        rb.constraints = RigidbodyConstraints2D.None;  //unlcok all contraint

    }

    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

 

    private void Update()
    {
        if (isDead) return;

        if (isWinning) return;

        moveInput = Input.GetAxisRaw("Horizontal");

       
        //rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);
        

        if (moveInput != 0)
        {
            transform.Rotate(0, 0, -moveInput * rotateSpeed * Time.deltaTime);
        }

        isGround = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x - 0.04290378f, gameObject.transform.position.y - 0.1225917f),
                                          checkSize, groundLayer);

        if (Input.GetKey(KeyCode.Space) && isGround && canJump)
        {
            jumpValue += 0.2f;

        }

        if (jumpValue >= 20f && isGround)
        {
            float tempXDir = moveInput * walkSpeed;
            float tempYDir = jumpValue;
            rb.velocity = new Vector2(tempXDir, tempYDir);
            REsetExplodeRange();
            Invoke("ResetJump", .2f);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isGround)
            {
                REsetExplodeRange();
                rb.velocity = new Vector2(moveInput * walkSpeed, jumpValue);
                jumpValue = 0;
            }
            canJump = true;
        }

        FillJumpBar();
        FixedBarUI();

        GravityManage();

        ExpandExplodeRadius();


    }

    [SerializeField]  private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 30f;

    private void FixedUpdate()
    {
        if (isGround)
        {
            // Direct velocity control on ground (responsive)
            rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);
        }
        else
        {
            // Limited air drift (tiny acceleration)
            rb.AddForce(new Vector2(moveInput * airControlForce, 0f));

          
        }
    }

    void Key_OnGetUnlockedKey(object sender, EventArgs e)
    {
        hasKey = true; 
    }

    void Bomb_OnGetBomb(object sender, EventArgs e)
    {
        hasBomb = true; 
    }

    private void FixedBarUI()
    {
        fillBar.transform.position = transform.position + new Vector3(0, 1.2f, 0);
        backGroundBar.transform.position = transform.position + new Vector3(0, 1.2f, 0);
    }

    private void ExpandExplodeRadius()
    {
        float t = jumpValue / 20f; // normalized 0–1
        currentExplodeRaidus = Mathf.Lerp(startRadius, maxRadius, t);
    }
    private void REsetExplodeRange()
    {

        Vector2 offset = new Vector2(transform.position.x - 0.0429f, transform.position.y - 0.1226f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(offset, currentExplodeRaidus, destroyObjLayer);

        foreach (var hit in hits)
        {
            Destroy(hit.gameObject);
        }

        currentExplodeRaidus = startRadius; 
    }

    private void GravityManage()
    {
        // Jump gravity control
        if (rb.velocity.y < 0)
        {
            //Higher gravity if falling
            SetGravityScale(gravityScale * fallGravityMult);
            //Caps maximum fall speed
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            //Default gravity if standing or moving upwards
            SetGravityScale(gravityScale);
        }
    }

    void ResetJump()
    {
        canJump = false;
        jumpValue = 0;
    }

    void FillJumpBar()
    {
        fillBar.fillAmount = jumpValue / 20f;
    }

    void DeathObj_OnPlayerDeath(object sender, EventArgs e)
    {
        GameObject deathEffect = Instantiate(this.deathEffect, transform.position, Quaternion.identity);
        Destroy(deathEffect, 1f);
    }

    void WinFlag_OnWinning(object sender, EventArgs e)
    {
        isWinning = true;
        transform.rotation = Quaternion.identity;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // remove contrain when start game
        anim.SetTrigger("Win");
    }


    private void OnDrawGizmos()
    {
        Vector2 offset = new Vector2(gameObject.transform.position.x - 0.04290378f, gameObject.transform.position.y - 0.1225917f);
        Gizmos.DrawWireSphere(offset, checkSize);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(offset, currentExplodeRaidus);
    }

    public bool HasKey() => hasKey;
    public bool HasBomb() => hasBomb;

    public bool IsDead() => isDead;

    public bool IsWin() => isWinning;

    public void SetDeath(bool value)
    {
        isDead = value;
    }


}

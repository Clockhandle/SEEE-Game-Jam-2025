using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float walkSpeed;
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



    private bool isNotRecoiling;


    [Header("KeyEvent")]
    UnclockKey key;
    private bool hasKey = false;

    [Header("Death")]
    bool isDead;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        key = FindObjectOfType<UnclockKey>();
        currentExplodeRaidus = startRadius;
        isNotRecoiling = true;

        key.OnGetUnlockkey += Key_OnGetUnlockedKey;
    }

    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    public void SetREcoil(bool value)
    {
        isNotRecoiling = value;
    }

    private void Update()
    {
        if (isDead) return;


        moveInput = Input.GetAxisRaw("Horizontal");

        if (isNotRecoiling) 
        {
            rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);
        }

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

    void Key_OnGetUnlockedKey(object sender, EventArgs e)
    {
        hasKey = true; 
    }

    private void FixedBarUI()
    {
        // keep UI upright
        fillBar.transform.rotation = Quaternion.identity;
        backGroundBar.transform.rotation = Quaternion.identity;
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
        if (rb.velocity.y < 0 && isNotRecoiling)
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

    
    

    private void OnDrawGizmos()
    {
        Vector2 offset = new Vector2(gameObject.transform.position.x - 0.04290378f, gameObject.transform.position.y - 0.1225917f);
        Gizmos.DrawWireSphere(offset, checkSize);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(offset, currentExplodeRaidus);
    }

    public bool HasKey() => hasKey;

    public bool IsDead() => isDead;

    public void SetDeath(bool value)
    {
        isDead = value;
    }
}

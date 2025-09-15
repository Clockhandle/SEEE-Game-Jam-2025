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

    public Image fillBar;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);
        if (moveInput != 0)
        {
            transform.Rotate(0, 0, -moveInput * rotateSpeed * Time.deltaTime);
        }

        isGround = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x - 0.04290378f, gameObject.transform.position.y - 0.1225917f),
                                          checkSize, groundLayer);

        if (Input.GetKey(KeyCode.Space) && isGround && canJump)
        {
            jumpValue += 0.1f;
        }

        if (jumpValue >= 20f && isGround)
        {
            float tempXDir = moveInput * walkSpeed;
            float tempYDir = jumpValue;
            rb.velocity = new Vector2(tempXDir, tempYDir);
            Invoke("ResetJump", .2f);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isGround)
            {
                rb.velocity = new Vector2(moveInput * walkSpeed,jumpValue);
                jumpValue = 0;
            }
            canJump = true;
        }

        FillJumpBar();
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
    }
}

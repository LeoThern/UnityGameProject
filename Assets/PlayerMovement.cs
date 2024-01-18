using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private bool isFacingRight = true;
    private bool onGround = false;

    [SerializeField] private float jumpingPower = 8f;
    [SerializeField] private float speed = 3f;
    [SerializeField][Range(0, 1)] float LerpConstant;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        CheckOnGround();
        CheckJump();
        CheckFlip();
    }

    private void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        Vector2 movement = new Vector2(speed * horizontal, rb.velocity.y);
        rb.velocity = Vector2.Lerp(rb.velocity, movement, LerpConstant);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void CheckOnGround()
    {
        bool newOnGround = IsGrounded();
        if (newOnGround && !onGround) onLanding();
        onGround = newOnGround;
    }

    private void CheckFlip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void CheckJump()
    {
        if (Input.GetButtonDown("Jump") && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.7f);
        }

        if (!onGround && rb.velocity.y < -0.01f)
        {
            print("is falling");
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", true);
        }
    }

    public void onLanding()
    {
        print("falling false");
        animator.SetBool("IsFalling", false);
    }
}

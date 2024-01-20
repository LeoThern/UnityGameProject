using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private bool isFacingRight = true;
    private bool onGround = false;
    private bool doesWeakAttack = false;
    private bool doesStrongAttack = false;
    private bool playerDoesEvade = false;

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
        CheckWeakAttack();
        CheckEvade();

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
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", true);
        }
    }

    private void CheckWeakAttack()
    {
        bool isAttacking = doesWeakAttack || doesStrongAttack;

        if (doesWeakAttack && !animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Idle_Weak_Attack"))
        {
            doesWeakAttack = false;
            animator.SetBool("DoesWeakAttack", false);
        }
        if (!isAttacking && Input.GetButtonDown("Weak Attack"))
        {
            doesWeakAttack = true;
            animator.SetBool("DoesWeakAttack", true);
        }
    }

    public void onLanding()
    {
        animator.SetBool("IsFalling", false);
    }

    private void CheckEvade()
    {
        if (playerDoesEvade && (!animator.GetCurrentAnimatorStateInfo(0).IsName("Running_Evade") || !animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Evade_Standing")))
        {
            playerDoesEvade = false;
            animator.SetBool("isEvading", false);
        }

        if (Input.GetButtonDown("Dodge") && onGround){
            playerDoesEvade = true;
            animator.SetBool("isEvading", true);
            
        }
    }
}

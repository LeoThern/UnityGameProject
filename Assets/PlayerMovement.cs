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
    private bool doesEvade = false;

    [SerializeField] private float jumpingPower = 8f;
    [SerializeField] private float speed = 3f;
    [SerializeField][Range(0, 1)] float lerpConstant;

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
        CheckEvade();
        CheckWeakAttack();
        CheckStrongAttack();
    }

    private void FixedUpdate()
    {
        horizontal = doesStrongAttack ? 0f : Input.GetAxisRaw("Horizontal");
        Vector2 movement = new Vector2(speed * horizontal, rb.velocity.y);
        rb.velocity = Vector2.Lerp(rb.velocity, movement, lerpConstant);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void CheckOnGround()
    {
        bool newOnGround = IsGrounded();
        if (newOnGround && !onGround) OnLanding();
        onGround = newOnGround;
    }

    private void CheckFlip()
    {
        if (doesStrongAttack) return;
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
        if (doesStrongAttack) return;
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

    private bool DoesWeakAttackAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Idle_Weak_Attack")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Running_Weak_Attack");
    }

    private void CheckWeakAttack()
    {
        bool isAttacking = doesWeakAttack || doesStrongAttack;

        if (doesWeakAttack && Input.GetButtonUp("Weak Attack"))
        {
            doesWeakAttack = false;
            animator.SetBool("DoesWeakAttack", false);
        }
        if (onGround && !isAttacking && Input.GetButtonDown("Weak Attack"))
        {
            doesWeakAttack = true;
            animator.SetBool("DoesWeakAttack", true);
        }
    }

    private void CheckStrongAttack()
    {
        if (onGround && !doesStrongAttack && Input.GetButtonDown("Strong Attack"))
        {
            doesStrongAttack = true;
            animator.SetBool("DoesStrongAttack", true);
        }
    }

    public void OnStrongAttackFinished()
    {
        doesStrongAttack = false;
        animator.SetBool("DoesStrongAttack", false);
        rb.position += new Vector2(HorizontalDirection() * 0.3f, 0f);
    }

    public void OnLanding()
    {
        animator.SetBool("IsFalling", false);
    }

    private void CheckEvade()
    {
        if (onGround && !doesEvade && Input.GetButtonDown("Dodge")){
            doesEvade = true;
            animator.SetBool("IsEvading", true);
            
        }
    }

    public void OnEvadeFinished()
    {
        doesEvade = false;
        animator.SetBool("IsEvading", false);
    }

    private float HorizontalDirection()
    {
        return isFacingRight ? 1f : -1f;
    }
}

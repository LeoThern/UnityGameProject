using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //inputs
    private float horizontal;
    private bool jumpPressed = false;
    private bool weakAttackPressed = false;
    private bool strongAttackPressed = false;
    private bool dodgePressed = false;

    private bool isFacingRight = true;
    private bool onGround = false;
    private bool doesWeakAttack = false;
    private bool doesStrongAttack = false;
    private bool doesEvade = false;
    private Vector2 movementInput = Vector2.zero;
    private float jumpTime = 0f;

    public HealthAndStamina healthAndStamina;
    /**
     * Usage:
     * healthAndStamina.checkAndConsumeStamina(20f)
     * 
     * returns true if there is enough Stanima
     * and if there is enough it automaticaly consumes the amount of stamina 
     */

    public float evadeCost = 10f;
    public float weakAttackCost = 3f;
    public float strongAttackCost = 40f;

    [SerializeField] public bool playerId = false;
    [SerializeField] private float jumpingPower = 8f;
    [SerializeField] private float minJumpTime = 1f;
    [SerializeField] private float dodgeAcceleration = 4f;
    [SerializeField] private float speed = 3f;
    [SerializeField][Range(0, 1)] float lerpConstant;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;

    [SerializeField] private CircleCollider2D enemyHitbox;
    [SerializeField] private BoxCollider2D enemyHurtbox;
    [SerializeField] private CircleCollider2D ownHitbox;
    [SerializeField] private BoxCollider2D ownHurtbox;
    [SerializeField] private PlayerMovement enemyMovement;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckOnGround();
        CheckJump();
        CheckFlip();
        CheckEvade();
        CheckWeakAttack();
        CheckStrongAttack();
        CheckHit();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        horizontal = movementInput.x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumpPressed = context.action.triggered;
    }

    public void OnWeakAttack(InputAction.CallbackContext context)
    {
        if (context.performed) // the key has been pressed
        {
            weakAttackPressed = !weakAttackPressed;
        }
    }

    public void OnStrongAttack(InputAction.CallbackContext context)
    {
        strongAttackPressed = context.ReadValueAsButton();
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        dodgePressed = context.ReadValueAsButton();
    }

    private void FixedUpdate()
    {
        horizontal = doesStrongAttack ? 0f : movementInput.x;
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
        if (newOnGround && !onGround && rb.velocity.y < 0.01f) OnLanding();
        onGround = newOnGround;
    }

    private void CheckJump()
    {
        if (doesStrongAttack) return;
        if (jumpPressed && onGround && rb.velocity.y < 0.01f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            jumpTime = 0f;
            animator.SetBool("IsJumping", true);
        }

        if (jumpPressed)
        {
            jumpTime += Time.deltaTime;
        }

        // check for slow down
        if (!jumpPressed && rb.velocity.y > 0f && jumpTime >= minJumpTime)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.75f * Time.deltaTime);
        }

        if (!onGround && rb.velocity.y < 0.01f)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", true);
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", false);
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

    private float HorizontalDirection()
    {
        return isFacingRight ? 1f : -1f;
    }

    private bool DoesWeakAttackAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Idle_Weak_Attack")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Running_Weak_Attack");
    }

    private void CheckWeakAttack()
    {
        bool isAttacking = doesWeakAttack || doesStrongAttack;

        if (doesWeakAttack && !weakAttackPressed)
        {
            doesWeakAttack = false;
            animator.SetBool("DoesWeakAttack", false);
        }
        if (onGround && !isAttacking && !doesEvade && weakAttackPressed)
        {
            doesWeakAttack = true;
            animator.SetBool("DoesWeakAttack", true);
        }
    }

    private void CheckStrongAttack()
    {
        if (onGround && !doesEvade && !doesStrongAttack && strongAttackPressed)
        {
            doesStrongAttack = true;
            animator.SetBool("DoesStrongAttack", true);
        }
    }

    public void OnStrongAttackFinished()
    {
        rb.position += new Vector2(HorizontalDirection() * 0.3f, 0f);
        doesStrongAttack = false;
        animator.SetBool("DoesStrongAttack", false);
    }

    private void CheckEvade()
    {
        if (dodgePressed && onGround && !doesEvade && healthAndStamina.checkAndConsumeStamina(evadeCost))
        {
            doesEvade = true;
            animator.SetBool("IsEvading", true);

            if (Mathf.Abs(rb.velocity.x) < 0.01) // player is idle
            {
                rb.position -= new Vector2(HorizontalDirection() * 1f, 0f);
            } else  // player is running
            {
                rb.velocity = rb.velocity = new Vector2(dodgeAcceleration * HorizontalDirection(), 0f);
            }
        }
    }

    private void CheckHit()
    {

        if (IsAttacking() && !enemyMovement.IsDodging() && enemyHitbox.IsTouching(ownHitbox))
        {
            print("hit!");
        }
    }

    public void OnEvadeFinished()
    {
        doesEvade = false;
        animator.SetBool("IsEvading", false);
    }

    public bool IsAttacking()
    {
        return doesWeakAttack || doesStrongAttack;
    }

    public bool IsDodging()
    {
        return doesEvade;
    }
}

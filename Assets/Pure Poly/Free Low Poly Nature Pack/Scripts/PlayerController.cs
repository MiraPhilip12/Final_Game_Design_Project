using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Forward Movement")]
    public float forwardSpeed = 8f;

    [Header("Lane Settings")]
    public float laneOffset = 2.5f;      // distance between lanes
    public float laneSwitchSpeed = 12f;

    [Header("Jump Settings")]
    public float jumpForce = 6.5f;

    [Header("Slide Settings")]
    public float slideDuration = 1.0f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;

    Rigidbody rb;
    Animator animator;

    int currentLane = 0; // -1, 0, +1
    float startX;
    float targetX;

    bool isSliding = false;
    bool isDead = false;

    /* =========================
       INITIALIZATION
       ========================= */

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        startX = transform.position.x;
        targetX = startX;

        rb.freezeRotation = true;
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    /* =========================
       INPUT
       ========================= */

    void OnMove(InputValue value)
    {
        if (isDead) return;

        float x = value.Get<Vector2>().x;

        if (x > 0.5f) ChangeLane(1);
        else if (x < -0.5f) ChangeLane(-1);
    }

    void OnJump()
    {
        if (isDead) return;
        if (!IsGrounded()) return;

        // Reset vertical velocity then jump
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        animator.SetTrigger("jumpTrigger");
    }

    void OnSlide()
    {
        if (isDead || isSliding || !IsGrounded()) return;
        StartCoroutine(SlideRoutine());
    }

    /* =========================
       LANE SWITCHING
       ========================= */

    void ChangeLane(int direction)
    {
        int newLane = Mathf.Clamp(currentLane + direction, -1, 1);
        if (newLane == currentLane) return;

        currentLane = newLane;
        targetX = startX + laneOffset * currentLane;
    }

    /* =========================
       PHYSICS & ANIMATION
       ========================= */

    void FixedUpdate()
    {
        if (isDead) return;

        /* ---- FORWARD RUNNING ---- */
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        /* ---- LANE MOVEMENT (X only) ---- */
        Vector3 pos = rb.position;
        float newX = Mathf.Lerp(pos.x, targetX, Time.fixedDeltaTime * laneSwitchSpeed);

        rb.MovePosition(new Vector3(newX, pos.y, pos.z) + forwardMove);

        /* ---- ANIMATION STATES ---- */
        bool grounded = IsGrounded();
        animator.SetBool("isGrounded", grounded);
        animator.SetFloat("verticalVelocity", rb.velocity.y);

        if (!grounded && rb.velocity.y < -0.1f)
            animator.SetBool("isFalling", true);
        else
            animator.SetBool("isFalling", false);
    }

    /* =========================
       SLIDE LOGIC
       ========================= */

    IEnumerator SlideRoutine()
    {
        isSliding = true;
        animator.SetBool("isSliding", true);

        yield return new WaitForSeconds(slideDuration);

        animator.SetBool("isSliding", false);
        isSliding = false;
    }

    /* =========================
       COLLISION → GAME OVER
       ========================= */

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool("isDead", true);

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
    }

    /* =========================
       GROUND CHECK
       ========================= */

    bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.05f;
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer);
    }
}

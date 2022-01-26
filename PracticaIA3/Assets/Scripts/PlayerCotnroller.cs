using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCotnroller : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5;

    [SerializeField]
    private float jumpForce = 5;

    [SerializeField]
    private float jumpTime = 0.25f;
    private float jumpTimeCounter = 0.25f;

    [SerializeField]
    public bool isJumping = false;

    [SerializeField]
    private bool isDoubleJumping = false;

    [SerializeField]
    private LayerMask maskGround;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private float groundCheckRadius;

    public bool buttomPress = false;

    public bool jumpingPrevious = false;

    private Collider2D col;
    private Rigidbody2D rb;
    private Animator animator;


    public float[] sensors;

    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        jumpTimeCounter = jumpTime;


    }

    void Update()
    {
        //isJumping = !Physics2D.IsTouchingLayers(col, maskGround);
        jumpingPrevious = isJumping;
        isJumping = !Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, maskGround);
        isDoubleJumping = isJumping ? isDoubleJumping : false;
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsDoubleJumping", isDoubleJumping);

        if(isJumping && !jumpingPrevious)
        {
            buttomPress = true;
        }
        else
        {
            buttomPress = false;
        }

        if (isDoubleJumping)
        {
            animator.SetBool("DoubleJumpDid", true);
        }
        if (!isJumping)
        {
            animator.SetBool("DoubleJumpDid", false);
        }

        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        if ((Input.GetButtonDown("Jump") || Input.GetButtonDown("Vertical")) && !isDoubleJumping)
        {
            buttomPress = true;
            isDoubleJumping = isJumping ? true : false;
            animator.SetBool("IsDoubleJumping", isDoubleJumping);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if ((Input.GetButton("Jump") || Input.GetButton("Vertical")) && !isDoubleJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
        }
        if ((Input.GetButtonUp("Jump") || Input.GetButtonUp("Vertical")))
        {
            buttomPress = false;
            jumpTimeCounter = 0;
        }
        if (!isJumping)
        {
            jumpTimeCounter = jumpTime;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "KillZone")
        {
            Gestor.singleton.RestartGame();
        }
    }
}

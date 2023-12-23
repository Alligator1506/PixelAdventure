using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TextMeshProUGUI pointText;

    private float horizontal;
    private bool isGrounded = true;

    private float speed = 7.0f;
    private float jumpForce = 14.0f;

    private int point;

    private bool canJump;

    [Header("Wall Jump")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    private bool canWallSlide;
    private bool isSliding;
    public float wallSlidingSpeed;
    private bool isWallDectected;


    // Update is called once per frame
    void Update()
    {
        isGrounded = CheckGrounded();
        isWallDectected = CheckWall();    
    }

    public override void OnInit()
    {
        base.OnInit();
        ChangeAnim("idle");

    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }

    public override void OnDeath()
    {
        base.OnDeath();
        ChangeAnim("death");
        Invoke(nameof(OnDespawn), 2f);
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                ChangeAnim("run");
            }
        }
        horizontal = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            TF.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
        }
        else if (isGrounded)
        {
            ChangeAnim("idle");
            rb.velocity = Vector2.zero;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CanJump();
        }

        if (!isGrounded && rb.velocity.y < 0f && !isWallDectected)
        {
            ChangeAnim("fall");
            canWallSlide = true;
        }


        if (isWallDectected && canWallSlide && !isGrounded)
        {
            isSliding = true;
            ChangeAnim("walljump");
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isSliding = false;
        } 
    }

    private void CanJump()
    {
        if (isSliding)
        {
            ChangeAnim("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = true;
        }
        else if (isGrounded)
        {
            Debug.Log("Jump lần 1");
            ChangeAnim("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = true;

        }
        else if (canJump)
        {
            Debug.Log("Jump lần 2");
            ChangeAnim("doublejump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = false;
        }
    }

    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        return hit.collider != null;
    }

    private bool CheckWall()
    {
        RaycastHit2D rcHit = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, wallLayer);

        return rcHit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.TAG_ITEMS)) {
            point++;
            pointText.text = "Point: " + point;
            Destroy(collision.gameObject);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(Constants.TAG_TRAP))
        {
            ChangeAnim("death");
            OnDeath();
        }
    }

}

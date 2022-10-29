using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the player's movement.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D cldr;
    private SpriteRenderer sprite;
    [SerializeField] private int direction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpReduction;
    [SerializeField] private float fallMultiplier;

    [Header("Ground & Wall Detection")]
    [SerializeField] private LayerMask isTerrain;
    [SerializeField] private float detectionRadius;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private List<Transform> wallChecks;
    [SerializeField] private bool onGround;
    [SerializeField] private bool onWall;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cldr = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();

        direction = (int)transform.right.x;
    }

    private void Update()
    {
        Move();
        Jump();
    }

    private void FixedUpdate()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, detectionRadius, isTerrain);
        onWall = wallChecks.Any(wallCheck => Physics2D.OverlapCircle(wallCheck.position, detectionRadius, isTerrain));

        // Fast fall
        if (rb.velocity.y < 0)
            rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
    }

    private void Move()
    {
        int dir = 0;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            dir = -1;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            dir = 1;
        }

        // Prevent getting stuck on walls
        if (dir == direction && onWall)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            if (direction == -dir) FlipX();
            rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);        
        }
    }

    private void Jump()
    {
        if (onGround && Input.GetKey(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        }

        // Stop jump short if button is released before apex
        if (Input.GetKeyUp(KeyCode.Space))
        {
            float jumpSpeed = jumpHeight / jumpReduction;
            if (jumpSpeed < rb.velocity.y)
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight / jumpReduction);
        }
    }

    // Flip the player horizontally.
    private void FlipX()
    {
        Vector3 oppDirection = transform.localScale;
        oppDirection.x *= -1;
        transform.localScale = oppDirection;
        direction = -direction;
    }
}

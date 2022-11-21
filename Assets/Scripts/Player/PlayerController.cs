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
    private HealthManager health;    

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float fallMultiplier;
    private int direction;

    [Header("Ground & Wall Detection")]
    [SerializeField] private LayerMask isTerrain;
    [SerializeField] private float groundRadius;
    [SerializeField] private float wallRadius;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private List<Transform> wallChecks;
    private bool onGround;
    private bool onWall;

    [Header("Combat")]
    [SerializeField] private BurstAttack attack;
    [SerializeField] private float attackCost;
    [SerializeField] private float boostSpeed;

    [SerializeField] [Tooltip("Prevent horizontal movement during boost.")] 
    private bool noMoveDuringBoost;

    [SerializeField] [Tooltip("Restricts boost to straight upwards propulsion.")]
    private bool upBoostOnly;

    [SerializeField] [Tooltip("Can only boost once while midair. Refreshes on ground.")]
    private bool oneTimeUseMidair;
    private bool canUseMidair;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cldr = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        health = GetComponent<HealthManager>();

        direction = (int)transform.right.x;
    }

    private void Update()
    {
        // Hot key for pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameStateManager.Instance.TogglePause();
        }

        // Player control
        if (!attack.inProgress)
        {
            Move();
            Jump();
            Attack();
        }
        else if (attack.inProgress && !noMoveDuringBoost)
        {
            Move();
        }
    }

    private void FixedUpdate()
    {
        onGround = CheckPlayerOnGround();
        onWall = CheckPlayerOnWall();
        canUseMidair = onGround || canUseMidair;

        // Fast fall
        if (rb.velocity.y < 0)
            rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
    }

    #region Detectors

    /// <summary>
    /// Checks if the ground detector is in contact with non-vertical terrain.
    /// </summary>
    private bool CheckPlayerOnGround()
    { 
        Collider2D[] cldrs = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius, isTerrain);

        // Check steepness of colliders relative to player
        if (cldrs.Length > 0)
        {
            return cldrs.Any(cldr => AdjustAngle(cldr.transform.eulerAngles.z) < 90);
        }

        return false;
    }

    /// <summary>
    /// Checks if any wall detector is in contact horizontal terrain.
    /// </summary>
    private bool CheckPlayerOnWall()
    {
        foreach (Transform wallCheck in wallChecks)
        {
            Collider2D cldr = Physics2D.OverlapCircle(wallCheck.position, wallRadius, isTerrain);

            if (cldr != null)
            {
                // Check steepness of collider relative to player
                if (AdjustAngle(cldr.transform.eulerAngles.z) > 0)
                    return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// Fit angle to be with 180 to -180 degree range.
    /// </summary>
    private float AdjustAngle(float a)
    {
        if (a >= 270)
        {
            a = 360 - a;
        }
        return a;
    }

    #endregion

    #region Movement

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
        if (dir == direction && onWall && !onGround)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            if (direction == -dir) FlipX();

            // Remove momentum when stopping on slants
            float vY = rb.velocity.y;
            if (dir == 0 && onGround && !attack.inProgress)
                vY = 0;

            rb.velocity = new Vector2(dir * moveSpeed, vY);
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
            if (rb.velocity.y > 0)
                rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    private void Attack()
    {
        // Pick between infinite boosting or ground-recharging boosts
        if (Input.GetKeyDown(KeyCode.S) && (canUseMidair || !oneTimeUseMidair))
        {
            health.ExpendFuel(attackCost);
            attack.Activate();
            canUseMidair = false;

            // Pick between vertical or omnidirectional boost
            if (upBoostOnly)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(Vector2.up * boostSpeed);
            }
            else
            {
                Vector2 thrust = rb.velocity.normalized;
                rb.velocity = Vector2.zero;
                rb.AddForce(thrust * boostSpeed);
            }
        }
    }

    #endregion

    // Flip the player horizontally.
    private void FlipX()
    {
        Vector3 oppDirection = transform.localScale;
        oppDirection.x *= -1;
        transform.localScale = oppDirection;
        direction *= -1;
    }   
}

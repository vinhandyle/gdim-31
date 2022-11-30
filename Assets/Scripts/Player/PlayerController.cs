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
    private HealthManager health;

    [Header("Movement")]
    [SerializeField] private GameObject cam;
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
    [SerializeField] private bool tiltWithGround;
    [SerializeField] private float maxGroundTilt;
    private bool onGround;
    private bool onWall;

    [Header("Combat")]
    [SerializeField] private BurstAttack attack;
    [SerializeField] private float attackCost;
    [SerializeField] private float boostSpeed;
    [SerializeField] private float stunTime;
    private bool stunned;

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
        health = GetComponent<HealthManager>();

        // Load saved position
        transform.position = new Vector3(
            SaveManager.Instance.playerData.posX, 
            SaveManager.Instance.playerData.posY, 
            transform.position.z)
            ;

        direction = (int)transform.right.x;        
        SaveManager.Instance.player = this;
    }

    private void Update()
    {
        UpdateCamera();

        // Hot key for pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameStateManager.Instance.TogglePause();
        }

        // Player control
        if (!attack.inProgress && !stunned && health.alive)
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
        List<float> angles = new List<float>();

        // Check steepness of colliders relative to player
        if (cldrs.Length > 0)
        {
            foreach (Collider2D cldr in cldrs)
            {
                angles.Add(AdjustAngle(cldr.transform.eulerAngles.z));
            }
        }

        angles = angles.Where(a => Mathf.Abs(a) < maxGroundTilt).ToList();

        // Update player tilt based on # of terrain colliders in contact
        if (tiltWithGround)
        {
            switch (angles.Count())
            {
                case 0:
                    transform.eulerAngles = Vector3.zero;
                    break;

                case 1:
                    transform.eulerAngles = new Vector3(0, 0, angles[0]);
                    break;
            }
        }

        return angles.Count() > 0;
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
                if (Mathf.Abs(AdjustAngle(cldr.transform.eulerAngles.z)) > 0)
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
            a -= 360;
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

            if (tiltWithGround)
            {
                // Special calc to move across slant
                float dX = transform.right.x;
                float dY = transform.right.y;
                float rot = transform.eulerAngles.z;

                // Flip vector if going down slant
                if (rot * dir < 0) dY *= -1;

                if (onGround && !attack.inProgress)
                    rb.velocity = new Vector2(dir * dX, Mathf.Abs(dir) * dY) * moveSpeed;
                else
                    rb.velocity = new Vector2(dir * moveSpeed, vY);
            }
            else
            {
                rb.velocity = new Vector2(dir * moveSpeed, vY);
            }            
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

    #region External

    /// <summary>
    /// Knockback the player by the specified force.
    /// Player is stunned during knockback to prevent momentum cancel.
    /// </summary>
    public IEnumerator Knockback(Vector2 kb)
    {
        stunned = true;
        rb.AddForce(kb);        

        yield return new WaitForSeconds(stunTime);

        stunned = false;
    }

    #endregion

    #region Helper

    /// <summary>
    /// Flip the player horizontally.
    /// </summary>
    private void FlipX()
    {
        Vector3 oppDirection = transform.localScale;
        oppDirection.x *= -1;
        transform.localScale = oppDirection;
        direction *= -1;
    }

    /// <summary>
    /// Lock the camera onto the player.
    /// </summary>
    private void UpdateCamera()
    {
        cam.transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                cam.transform.position.z
            );
    }

    #endregion
}

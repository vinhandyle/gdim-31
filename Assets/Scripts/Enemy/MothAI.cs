using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The behavior of the moth enemy.
/// </summary>
public class MothAI : BreakableObject
{
    [SerializeField] protected Vector2 moveDirection;
    [SerializeField] protected float moveSpeed;
    private int faceDirection;

    [SerializeField] private Sprite spriteToScaleTo;
    [SerializeField] private List<Collider2D> bounds;

    protected Rigidbody2D rb;
    protected SpriteRenderer sprite;

    protected override void Awake()
    {
        base.Awake();

        faceDirection = transform.localScale.x.CompareTo(0);
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hostile"))
        {
            // Stop and spawn toxic gas
            rb.velocity = Vector2.zero;
        }
        else
        {
            // Patrol
            rb.velocity = moveSpeed * moveDirection;
        }

        AdjustScale();
    }

    #region Aggro

    /// <summary>
    /// Make the sprite face towards the player.
    /// </summary>
    private void FacePlayer(Transform player)
    {
        if (player.position.x.CompareTo(transform.position.x) != faceDirection)
        {
            FlipX();
        }
    }

    #endregion

    #region Sprite

    /// <summary>
    /// Scale the transform so that the dimensions are consistent with the base sprite.
    /// Useful for animation states with different dimensions.
    /// </summary>
    protected void AdjustScale()
    {
        Sprite _sprite = sprite.sprite;

        float unitLength = spriteToScaleTo.bounds.size.y * spriteToScaleTo.pixelsPerUnit;
        float w = _sprite.bounds.size.x * _sprite.pixelsPerUnit / unitLength;
        float h = _sprite.bounds.size.y * _sprite.pixelsPerUnit / unitLength;
        //Debug.Log((w * unitLength) + "x" + (h * unitLength));

        sprite.size = new Vector2(w * faceDirection, h);
        ((BoxCollider2D)hitbox).size = new Vector2(w, h);
    }

    /// <summary>
    /// Flip the moth horizontally.
    /// </summary>
    protected void FlipX()
    {
        Vector3 oppDirection = transform.localScale;
        oppDirection.x *= -1;
        transform.localScale = oppDirection;

        moveDirection.x *= -1;
        faceDirection *= -1;
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("Player In Range", true);
        }
        
        // Turn around if attempting to leave boundary
        if (bounds.Any(c => c == collision))
        {
            if (moveDirection.x != 0) FlipX();
            moveDirection.y *= -1;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FacePlayer(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("Player In Range", false);
        }
    }
}

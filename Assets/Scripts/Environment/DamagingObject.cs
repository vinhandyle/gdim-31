using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows an object to damage the player on contact.
/// </summary>
public class DamagingObject : MonoBehaviour
{
    private Animator anim;
    private Collider2D cldr;
    private Rigidbody2D rb;
    private MovingObject mvObj;

    [SerializeField] private int damage;
    [SerializeField] private bool instantKill;
    [SerializeField] private bool destroyOnHit;
    [SerializeField] private bool destroyOnBurst;
    [SerializeField] private bool canKnockback;
    [SerializeField] private float knockbackAmt;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        cldr = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        mvObj = GetComponent<MovingObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cldr.isTrigger) OnContact(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!cldr.isTrigger) OnContact(collision.gameObject);
    }

    private void OnContact(GameObject other)
    {
        // Damage player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            HealthManager health = other.GetComponent<HealthManager>();

            if (instantKill)
            {
                health.Die();              
            }
            else
            {
                health.TakeDamage(damage);

                if (canKnockback)
                {                    
                    Vector2 kb = (other.transform.position - transform.position) * knockbackAmt;
                    StartCoroutine(player.Knockback(kb));
                }
            }
        }

        // Destroy object if appropriate
        bool hitByBurst = other.GetComponent<BurstAttack>();
        if ((!hitByBurst && destroyOnHit) || (hitByBurst && destroyOnBurst))
        {
            if (mvObj != null) mvObj.enabled = false;

            cldr.enabled = false;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;

            // Play collision animation
            if (anim == null)
                Destroy(gameObject);
            else
                anim.SetTrigger("OnDestroy");
        }
    }

    /// <summary>
    /// Call to destroy object after collision animation finishes.
    /// </summary>
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}

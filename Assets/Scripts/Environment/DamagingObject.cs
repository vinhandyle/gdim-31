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

    private HealthManager health;
    private bool applyFriction;
    private float frictionTime;
    private float frictionTimeMax = 0.5f;

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

        frictionTime = frictionTimeMax;
    }

    private void Update()
    {
        if (health != null && health.alive)
        {
            applyFriction = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

            // Deal damage on initial move
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                health.TakeDamage(1);
            }

            // Deal damage every half second while moving
            if (applyFriction)
            {
                frictionTime -= Time.deltaTime;
                if (frictionTime <= 0)
                {
                    health.TakeDamage(1);
                    frictionTime = frictionTimeMax;
                }
            }

            // Reset friction when stop moving
            if (!applyFriction) frictionTime = frictionTimeMax;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cldr.isTrigger) OnContact(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!cldr.isTrigger) OnContact(collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            health = null;
            applyFriction = false;
            frictionTime = frictionTimeMax;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnContact(GameObject other)
    {
        // Damage player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            health = other.GetComponent<HealthManager>();

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

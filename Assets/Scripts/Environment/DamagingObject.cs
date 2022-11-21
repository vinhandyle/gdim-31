using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows an object to damage the player on contact.
/// </summary>
public class DamagingObject : MonoBehaviour
{
    private bool isTrigger;

    [SerializeField] private int damage;
    [SerializeField] private bool instantKill;
    [SerializeField] private bool destroyOnHit;
    [SerializeField] private bool destroyOnBurst;
    [SerializeField] private bool canKnockback;
    [SerializeField] private float knockbackAmt;

    private void Awake()
    {
        isTrigger = GetComponent<Collider2D>().isTrigger;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTrigger) OnContact(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isTrigger) OnContact(collision.gameObject);
    }

    private void OnContact(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            HealthManager health = other.GetComponent<HealthManager>();

            if (instantKill)
            {
                // Change to have custom death animation
                health.Respawn();
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

        bool hitByBurst = other.GetComponent<BurstAttack>();
        if ((!hitByBurst && destroyOnHit) || (hitByBurst && destroyOnBurst))
        {
            Destroy(gameObject);
        }
    }
}

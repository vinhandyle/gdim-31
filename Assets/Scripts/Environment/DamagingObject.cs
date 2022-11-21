using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows an object to damage the player on contact.
/// </summary>
public class DamagingObject : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private bool instantKill;
    [SerializeField] private bool destroyOnHit;
    [SerializeField] private bool destroyOnBurst;
    [SerializeField] private bool canKnockback;

    private Animator anim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnContact(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnContact(collision.gameObject);
    }

    private void OnContact(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager health = other.GetComponent<HealthManager>();

            if (instantKill)
            {
                // Change to have custom death animation
                anim.SetBool("Death", true);
                //health.Respawn();
                
            }
            else
            {
                health.TakeDamage(damage);

                if (canKnockback)
                {
                    // TODO
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

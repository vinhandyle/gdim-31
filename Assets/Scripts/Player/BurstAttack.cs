using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The AoE attack centralized on the player.
/// </summary>
public class BurstAttack : MonoBehaviour
{
    private Collider2D hitbox;
    private Animator anim;

    public bool inProgress { get => anim.GetBool("Active"); }


    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        hitbox.enabled = false;
    }

    /// <summary>
    /// Begin the attack.
    /// </summary>
    public void Activate()
    {
        anim.SetBool("Active", true);
        AudioController.Instance.PlayEffect(0);
    }

    /// <summary>
    /// Turn the hitbox on during the climax of the animation. 
    /// </summary>
    private void EnableHitBox()
    {
        hitbox.enabled = true;
    }

    /// <summary>
    /// Finish the attack.
    /// </summary>
    private void Deactivate()
    {
        anim.SetBool("Active", false);
        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BreakableObject obj = collision.GetComponent<BreakableObject>();
        if (obj != null)
        {
            obj.Break();
        }
    }
}

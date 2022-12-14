using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that can be destroyed by the player's burst attack.
/// </summary>
public class BreakableObject : MonoBehaviour
{
    [SerializeField] protected Collider2D hitbox;
    protected Animator anim;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Unload the object's hitbox and start its breaking animation.
    /// </summary>
    public void Break()
    {
        hitbox.enabled = false;
        anim.SetBool("", true);
        BreakFinish(); // Use if break animation is unfinished
    }

    /// <summary>
    /// Unload or destroy the object after the breaking animation finishes.
    /// </summary>
    private void BreakFinish()
    {
        gameObject.SetActive(false);
        // Destroy(gameObject); // If we don't want to reload object on respawn
    }
}

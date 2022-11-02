using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that can be destroyed by the player's burst attack.
/// </summary>
public class BreakableObject : MonoBehaviour
{    
    private Animator anim;
    private Collider2D cldr;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        cldr = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Unload the object's hitbox and start its breaking animation.
    /// </summary>
    public void Break()
    {
        cldr.enabled = false;
        anim.SetBool("", true);
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

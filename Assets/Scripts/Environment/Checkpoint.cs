using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that the player can respawn at.
/// </summary>
public class Checkpoint : InteractableObject
{
    private Animator anim;

    public bool active { get => anim.GetBool("Enabled"); }
    [SerializeField] private bool preset;

    protected override void Awake()
    {
        base.Awake();

        anim = GetComponent<Animator>();

        // Add preset checkpoints
        OnEnterRange += (player) =>
        {
            if (preset)
            {
                HealthManager playerHealth = player.GetComponent<HealthManager>();

                anim.SetBool("Enabled", true);
                anim.SetBool("OnCheckpoint", true);
                playerHealth.AddCheckpoint(this);
            }
        };

        // Activate checkpoint on first interaction
        // Full heal on subsequent interactions
        OnInteract += (player) => 
        {
            HealthManager playerHealth = player.GetComponent<HealthManager>();
           
            if (active)
            {
                playerHealth.Respawn();
            }
            playerHealth.AddCheckpoint(this);
            //anim.SetBool("Enabled", true);
            
        };
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that the player can respawn at.
/// </summary>
public class Checkpoint : InteractableObject
{
    public bool active { get; private set; }
    [SerializeField] private bool preset;

    protected override void Awake()
    {
        base.Awake();

        // Add preset checkpoints
        OnEnterRange += (player) =>
        {
            if (preset)
            {
                HealthManager playerHealth = player.GetComponent<HealthManager>();

                active = true;
                playerHealth.AddCheckpoint(this);
            }
        };

        // Activate checkpoint on first interaction
        // Full heal on subsequent interactions
        OnInteract += (player) => 
        {
            HealthManager playerHealth = player.GetComponent<HealthManager>();

            active = true;
            playerHealth.AddCheckpoint(this);

            if (active)
            {
                playerHealth.Respawn();
            }
        };
    }
}

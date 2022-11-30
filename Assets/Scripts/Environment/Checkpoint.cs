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

    public int checkpointID { get => _checkpointID; }
    [SerializeField] private int _checkpointID;

    protected override void Awake()
    {
        base.Awake();

        anim = GetComponent<Animator>();
        intTexts.Add("Press [W] to light");
        intTexts.Add("Press [W] to refuel");

        // Add preset checkpoints
        OnEnterRange += (player) =>
        {
            if (preset)
            {
                HealthManager playerHealth = player.GetComponent<HealthManager>();
                playerHealth.SetCheckpoint(checkpointID);
                Activate();
            }
        };

        // Activate checkpoint on first interaction
        // Full heal on subsequent interactions
        OnInteract += (player) => 
        {
            HealthManager playerHealth = player.GetComponent<HealthManager>();                        
            playerHealth.SetCheckpoint(checkpointID);

            if (active)
            {
                playerHealth.Die("deathMelt");
            }
            else
            {
                Activate();
                AudioController.Instance.PlayEffect(4);
            }           
        };
    }

    /// <summary>
    /// Find and return the checkpoint with the specified id in the current scene.
    /// Return null if the checkpoint cannot be found.
    /// </summary>
    public static Checkpoint Find(int id)
    {
        foreach (Checkpoint checkpoint in FindObjectsOfType<Checkpoint>())
        {
            if (checkpoint.checkpointID == id) return checkpoint;
        }
        return null;
    }

    /// <summary>
    /// Reactivate all checkpoints in the scene that were activated by the
    /// player during this playthrough.
    /// </summary>
    public static void ReactivateAll(List<int> ids)
    {
        foreach (Checkpoint checkpoint in FindObjectsOfType<Checkpoint>())
        {
            if (ids.Contains(checkpoint.checkpointID)) checkpoint.Activate(); 
        }
    }

    /// <summary>
    /// Change the checkpoint's animation to the Enabled state.
    /// </summary>
    private void Activate()
    {
        anim.SetBool("Enabled", true);
        intTextInd = 1;
    }
}

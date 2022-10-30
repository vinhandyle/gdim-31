using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks and updates the player's health.
/// </summary>
public class HealthManager : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();

    [SerializeField] private float maxTime;
    [SerializeField] private float decaySpeed;
    [SerializeField] [Range(1, 100)] private int healthSegments = 1;

    private float timeLeft;
    private float timePerHealth;
    private int dmgTaken;

    private void Awake()
    {
        timeLeft = maxTime;
        timePerHealth = maxTime / healthSegments;
        healthBar.SetDefaults(maxTime, healthSegments);
    }

    private void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime * decaySpeed;
            healthBar.SetHealth(timeLeft);
        }
        else
        {
            Respawn();
        }
    }

    public void TakeDamage(int amt)
    {
        dmgTaken += amt;
        healthBar.SetDamagedHealth(dmgTaken);
        if (dmgTaken * timePerHealth >= timeLeft)
            timeLeft = 0;
    }

    /// <summary>
    /// Add the given checkpoint to the list of ones
    /// available for respawn.
    /// </summary>
    public void AddCheckpoint(Checkpoint checkpoint)
    {
        checkpoints.Add(checkpoint);
    }

    /// <summary>
    /// Respawn the player at the most recently activated checkpoint.
    /// Modify logic to determine checkpoint?
    /// </summary>
    public void Respawn()
    {
        Vector3 target = checkpoints[checkpoints.Count - 1].transform.position;
        transform.position = new Vector3(target.x, target.y, transform.position.z);

        timeLeft = maxTime;
        healthBar.SetHealth(maxTime);
        healthBar.ResetDamagedHealth();
    }
}

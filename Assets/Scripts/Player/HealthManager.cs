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

    [SerializeField] private bool disableHealthDrain;
    [SerializeField] private float maxTime;
    [SerializeField] private float decaySpeed;
    private float baseDecaySpeed;
    [SerializeField] [Range(1, 100)] private int healthSegments = 1;

    private float timeLeft;
    private float timePerHealth;
    private int dmgTaken;

    private void Awake()
    {
        timeLeft = maxTime;
        timePerHealth = maxTime / healthSegments;
        baseDecaySpeed = decaySpeed;
        healthBar.SetDefaults(maxTime, healthSegments);
    }

    private void Update()
    {
        if (timeLeft > 0)
        {
            if (!disableHealthDrain)
                timeLeft -= Time.deltaTime * decaySpeed;
            healthBar.SetHealth(timeLeft);
        }
        else
        {
            Respawn();
        }
    }

    /// <summary>
    /// Set the health decay speed multiplier.
    /// </summary>
    public void ChangeDecaySpeed(float mult)
    {
        decaySpeed = baseDecaySpeed * mult;
    }

    /// <summary>
    /// Reduce the amount of time left.
    /// </summary>
    public void ExpendFuel(float amt = -1)
    {
        if (amt < 0)
            timeLeft = 0;
        timeLeft -= amt;
    }

    /// <summary>
    /// Deal damage to the candle stick.
    /// If total damage exceeds time left, the player dies.
    /// </summary>
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
        Checkpoint target = checkpoints[checkpoints.Count - 1];
        Vector3 dest = target.transform.position + (Vector3)target.GetComponent<BoxCollider2D>().offset;
        transform.position = new Vector3(dest.x, dest.y, transform.position.z);

        timeLeft = maxTime;
        healthBar.SetHealth(maxTime);
        healthBar.ResetDamagedHealth();
    }
}

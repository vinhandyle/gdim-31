using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks and updates the player's health.
/// </summary>
public class HealthManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private HealthBar healthBar;
    public int respawnPoint { get; private set; }

    [SerializeField] private bool disableHealthDrain;
    [SerializeField] private float maxTime;
    [SerializeField] private float decaySpeed;
    private float baseDecaySpeed;
    [SerializeField] [Range(1, 100)] private int healthSegments = 1;

    private Rigidbody2D rb;
    public bool alive { get => rb.bodyType == RigidbodyType2D.Dynamic; }

    private float timePerHealth;
    public float timeLeft { get; private set; }

    public int dmgTaken { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        timeLeft = maxTime;
        timePerHealth = maxTime / healthSegments;
        baseDecaySpeed = decaySpeed;
        healthBar.SetDefaults(maxTime, healthSegments);
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player Idle"))
        {
            if (timeLeft > 0)
            {
                if (!disableHealthDrain)
                    timeLeft -= Time.deltaTime * decaySpeed;
                healthBar.SetHealth(timeLeft);
            }
            else
            {
                Die("deathMelt");
            }
        }
    }

    /// <summary>
    /// Set the current time left and damage taken.
    /// </summary>
    public void SetHealthBar(float timeLeft, int dmgTaken)
    {
        this.timeLeft = timeLeft;
        this.dmgTaken = dmgTaken;
        healthBar.SetDamagedHealth(dmgTaken);
        healthBar.SetHealth(timeLeft);
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
        AudioController.Instance.PlayEffect(1);

        if (dmgTaken * timePerHealth >= timeLeft)
        {
            Die("deathBreak");
        }
    }

    /// <summary>
    /// Set the player's respawn point to the specified checkpoint.
    /// </summary>
    public void SetCheckpoint(int id)
    {
        respawnPoint = id;
        SaveManager.Instance.SaveCheckpoint(id);
    }

    /// <summary>
    /// Restricts player movement and begins one of three death animations.
    /// </summary>
    public void Die(string deathType = "deathExt")
    {
        rb.bodyType = RigidbodyType2D.Static; // Prevent movement during death & respawn
        PlayDeathSound(deathType);

        // Melt is checked continuously so trigger is not good
        if (deathType == "deathMelt")
        {
            anim.SetBool(deathType, true);
        }
        else
        {
            anim.SetTrigger(deathType);
        }
    }

    /// <summary>
    /// Play a sound effect based on death type.
    /// </summary>
    private void PlayDeathSound(string deathType)
    {
        switch (deathType)
        {
            case "deathExt":
                AudioController.Instance.PlayEffect(3);
                break;

            case "deathMelt":
                AudioController.Instance.PlayEffect(3);
                break;

            case "deathBreak":
                AudioController.Instance.PlayEffect(2);
                break;
        }
    }

    /// <summary>
    /// Respawn the player at the most recently used checkpoint.
    /// </summary>
    public void Respawn()
    {
        // If last checkpoint is in different scene
        if (SaveManager.Instance.LoadRespawn(respawnPoint)) return;

        // If last checkpoint is in same scene
        rb.bodyType = RigidbodyType2D.Static; // Prevent movement during cross-scene respawn

        Checkpoint target = Checkpoint.Find(respawnPoint);
        Vector3 dest = target.transform.position + (Vector3)target.GetComponent<BoxCollider2D>().offset;
        transform.position = new Vector3(dest.x, dest.y, transform.position.z);        
        anim.SetBool("deathMelt", false);
        anim.SetTrigger("onRespawn");
        AudioController.Instance.PlayEffect(5);
    }

    /// <summary>
    /// Reset player health and unlock movement.
    /// Call when respawn animation finishes.
    /// </summary>
    public void RespawnFinish()
    {
        timeLeft = maxTime;
        dmgTaken = 0;
        healthBar.SetHealth(maxTime);
        healthBar.ResetDamagedHealth();
        rb.bodyType = RigidbodyType2D.Dynamic; // Return player control        
    }
}

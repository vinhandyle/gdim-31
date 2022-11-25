using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Intermediary script for player animation events.
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private HealthManager health;

    public void Respawn()
    {
        health.Respawn();
    }

    public void RespawnFinish()
    {
        health.RespawnFinish();
    }
}

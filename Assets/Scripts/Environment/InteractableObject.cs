using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that the player can "Interact" with.
/// </summary>
public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] protected PlayerController player = null;
    [SerializeField] protected bool playerInRange = false;

    protected event Action<PlayerController> OnInteract;
    protected event Action<Collider2D> OnEnterRange;
    protected event Action<Collider2D> OnExitRange;

    protected virtual void Awake()
    {
        OnEnterRange += (collision) =>
        {
            player = collision.GetComponentInParent<PlayerController>();
            playerInRange = true;
        };

        OnExitRange += (collision) =>
        {
            playerInRange = false;
        };
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && playerInRange)
        {
            OnInteract?.Invoke(player);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnEnterRange?.Invoke(collision);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnExitRange?.Invoke(collision);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Expanding area that accelerates the player's health decay.
/// </summary>
public class ToxicGas : MonoBehaviour
{
    [SerializeField] [Range(1, 100)] private float decayAccel;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    private float curSize;
    [SerializeField] private float durationAfterMaxSize;
    private float aiTimer;

    public event Action OnDestroyEvent;

    private void Awake()
    {
        transform.localScale = Vector2.zero;
    }

    private void Update()
    {
        if (curSize < maxSize)
        {
            curSize += Time.deltaTime * growSpeed;
        }
        else
        {
            aiTimer += Time.deltaTime;
            if (aiTimer >= durationAfterMaxSize)
            {               
                Destroy(gameObject);
            }
        }

        if (curSize > maxSize)
        {
            curSize = maxSize;           
        }

        transform.localScale = Vector2.one * curSize;
    }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<HealthManager>().ChangeDecaySpeed(decayAccel);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<HealthManager>().ChangeDecaySpeed(1);
        }
    }
}

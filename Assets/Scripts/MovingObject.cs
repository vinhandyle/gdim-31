using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows an object to move in a pre-determined manner.
/// </summary>
public class MovingObject : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private Vector2 direction;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float aiTimer;

    [Header("Modes")]
    [SerializeField] private Mode mode;
    private enum Mode
    {
        Infinite,
        LineCycle
    }

    [SerializeField] private float cycleTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetVelocity();

        aiTimer = cycleTime;
    }

    private void Update()
    {
        aiTimer -= Time.deltaTime;
        if (aiTimer <= 0)
        {
            switch (mode)
            {
                case Mode.LineCycle:
                    direction = -direction;
                    break;
            }
            
            SetVelocity();
            aiTimer = cycleTime;
        }
    }

    private void SetVelocity()
    {
        rb.velocity = direction.normalized * moveSpeed;
    }
}

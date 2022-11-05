using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that loads a preset scene on interaction.
/// </summary>
public class LevelConnector : InteractableObject
{
    [SerializeField] private string nextLevel;

    protected override void Awake()
    {
        base.Awake();

        OnInteract += (player) =>
        {
            SceneController.Instance.LoadScene(nextLevel);
        };
    }
}

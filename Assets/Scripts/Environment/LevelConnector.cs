using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that loads a preset scene on interaction.
/// </summary>
public class LevelConnector : InteractableObject
{
    [SerializeField] private string nextLevel;
    [SerializeField] private Vector2 nextPos;

    protected override void Awake()
    {
        base.Awake();

        OnInteract += (player) =>
        {
            SaveManager.Instance.SavePlayerInfo(SceneController.Instance.currentScene);
            SaveManager.Instance.LoadLevel(nextLevel, nextPos);
        };
    }
}

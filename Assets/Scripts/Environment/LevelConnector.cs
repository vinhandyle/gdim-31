using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that loads a preset scene on interaction.
/// </summary>
public class LevelConnector : InteractableObject
{
    private Animator anim;
    private Collider2D cldr;

    [SerializeField] private string nextLevel;
    [SerializeField] private Vector2 nextPos;
    [SerializeField] private bool delayForAnimation;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        cldr = GetComponent<Collider2D>();

        intTexts.Add("Press [W] to enter");
        intTexts.Add("Press [W] to light");

        OnInteract += (player) =>
        {
            if (delayForAnimation)
            {
                player.GetComponent<HealthManager>().disableHealthDrain = true;

                anim.SetTrigger("OnInteract");
                cldr.enabled = false;   
                AudioController.Instance.PlayEffect(6);
            }
            else
            {
                GoToNextLevel();
            }
        };
    }

    private void GoToNextLevel()
    {
        SaveManager.Instance.SavePlayerInfo(SceneController.Instance.currentScene);
        SaveManager.Instance.LoadLevel(nextLevel, nextPos);
    }
}

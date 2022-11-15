using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTrigger : InteractableObject
{
    [SerializeField] private GameObject text;
    [SerializeField] private bool permanent;

    protected override void Awake()
    {
        base.Awake();

        text.SetActive(permanent);

        OnEnterRange += (collision) => 
        {
            if (!permanent)
                text.SetActive(true);
        };

        OnExitRange += (collision) =>
        {
            if (!permanent)
                text.SetActive(false);
        };
    }
}

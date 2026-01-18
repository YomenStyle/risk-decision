using System;
using UnityEngine;

[Serializable]
public class ActionData
{
    [SerializeField] private float successChance = 0.7f;
    [SerializeField] private ActionEffect onSuccessEffect = new ActionEffect { TargetHpDelta = -2 };
    [SerializeField] private ActionEffect onFailureEffect = new ActionEffect { ActorHpDelta = -1 };

    public float SuccessChance
    {
        get => successChance;
        set => successChance = value;
    }

    public ActionEffect OnSuccessEffect
    {
        get => onSuccessEffect;
        set => onSuccessEffect = value;
    }

    public ActionEffect OnFailureEffect
    {
        get => onFailureEffect;
        set => onFailureEffect = value;
    }
}

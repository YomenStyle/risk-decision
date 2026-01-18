using System;
using UnityEngine;

[Serializable]
public class ActionEffect
{
    [SerializeField] private int actorHpDelta;
    [SerializeField] private int targetHpDelta;

    public int ActorHpDelta
    {
        get => actorHpDelta;
        set => actorHpDelta = value;
    }

    public int TargetHpDelta
    {
        get => targetHpDelta;
        set => targetHpDelta = value;
    }

    public void Apply(Combatant actor, Combatant target)
    {
        if (actor != null)
        {
            actor.ApplyHpDelta(actorHpDelta);
        }

        if (target != null)
        {
            target.ApplyHpDelta(targetHpDelta);
        }
    }
}

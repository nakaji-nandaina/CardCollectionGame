using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActiveStatusEffect
{
    public StatusEffectType StatusEffectType;
    public int RemainingTurns;
    
    public ActiveStatusEffect(StatusEffectType type, int turns = 0)
    {
        StatusEffectType = type;
        RemainingTurns = turns;
    }
}

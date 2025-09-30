using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
    Poison,
    Burn,
    Freeze,
    Paralysis,
    Bleed,
    Confusion,
    Stun
}

[Serializable]
public class EffectResistances
{
    [Header("耐性(パーセント)")]
    [SerializeField, Header("毒"), Range(-100,1000)] int _poison = 0;
    [SerializeField, Header("火傷"), Range(-100,1000)] int _burn = 0;
    [SerializeField, Header("凍結"), Range(-100, 1000)] int _freeze = 0;
    [SerializeField, Header("麻痺"), Range(-100, 1000)] int _paralysis = 0;
    [SerializeField, Header("出血"), Range(-100, 1000)] int _bleed = 0;
    [SerializeField, Header("混乱"), Range(-100, 1000)] int _confusion = 0;
    [SerializeField, Header("スタン"), Range(-100, 1000)] int _stun = 0;

    public int Poison => _poison;
    public int Burn => _burn;
    public int Freeze => _freeze;
    public int Paralysis => _paralysis;
    public int Bleed => _bleed;
    public int Confusion => _confusion;
    public int Stun => _stun;
}

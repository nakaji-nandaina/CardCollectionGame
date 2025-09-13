using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HealSkillData : BaseSkillData
{
    [SerializeField, Header("ƒx[ƒX‰ñ•œ—Ê")] private int _baseHealAmount;
    [SerializeField, Header("UŒ‚—Í”{—¦")] private float _healPowerRate;
    public int BaseHealAmount => _baseHealAmount;
    public float HealPowerRate => _healPowerRate;
}

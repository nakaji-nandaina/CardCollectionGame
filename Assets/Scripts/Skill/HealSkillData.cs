using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HealSkillData : BaseSkillData
{
    [SerializeField, Header("�x�[�X�񕜗�")] private int _baseHealAmount;
    [SerializeField, Header("�U���͔{��")] private float _healPowerRate;
    public int BaseHealAmount => _baseHealAmount;
    public float HealPowerRate => _healPowerRate;
}

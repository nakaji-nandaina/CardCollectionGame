using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseSkillData
{
    [SerializeField, Header("�X�L����")] protected string _skillName;
    [SerializeField, Header("�K�vMP")] protected int _mpCost;
    [SerializeField, Header("�X�L���A�j���[�V�����^�C�v")] protected BattleUIfxType skillFxType;

    public string SkillName => _skillName;
    public int MpCost => _mpCost;
    public BattleUIfxType SkillFxType => skillFxType;
}

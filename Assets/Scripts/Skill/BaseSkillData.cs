using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseSkillData
{
    [SerializeField, Header("スキル名")] protected string _skillName;
    [SerializeField, Header("必要MP")] protected int _mpCost;

    public string SkillName => _skillName;
    public int MpCost => _mpCost;
}

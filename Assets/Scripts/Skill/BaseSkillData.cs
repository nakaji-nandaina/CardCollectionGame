using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseSkillData
{
    [SerializeField, Header("�X�L����")] protected string _skillName;
    [SerializeField, Header("�K�vMP")] protected int _mpCost;

    public string SkillName => _skillName;
    public int MpCost => _mpCost;
}

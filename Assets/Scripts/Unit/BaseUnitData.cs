using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum UnitType
{
    Player,
    Enemy
}

public abstract class BaseUnitData : BaseScriptableObject
{
    [SerializeField, Header("名前")] private string _unitName;
    [SerializeField, Header("全身画像")] private Sprite _fullSprite;
    [SerializeField, Header("顔画像")] private Sprite _faceSprite;
    [SerializeField, Header("体力")] private int _maxHealth;
    [SerializeField, Header("攻撃力")] private int _attackPoint;
    [SerializeField, Header("魔法防御力")] private int _magicDefensePoint;
    [SerializeField, Header("物理防御力")] private int _physDefensePoint;
    [SerializeReference, SelectableSerializeReference, Header("スキル")] private List<BaseSkillData> _skills;

    public string UnitName=> _unitName;
    public Sprite FullSprite => _fullSprite;
    public Sprite FaceSprite => _faceSprite;
    public int MaxHealth => _maxHealth;
    public int AttackPoint => _attackPoint;
    public int MagicDefensePoint => _magicDefensePoint;
    public int PhysDefensePoint => _physDefensePoint;
    public List<BaseSkillData> Skills => _skills;
    public abstract UnitType UnitType { get;  }
}

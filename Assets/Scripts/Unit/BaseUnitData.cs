using System.Collections.Generic;
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
    [SerializeField, Header("魔力")] private int _maxMagicPoint;
    [SerializeField, Header("攻撃力")] private int _attackPoint;
    [SerializeField, Header("魔法防御力")] private int _magicDefensePoint;
    [SerializeField, Header("物理防御力")] private int _physDefensePoint;
    [SerializeField, Header("速度"), Range(1, 200)] private int _speed = 10;
    [SerializeField, Header("状態異常耐性")] private EffectResistances _effectResistances;
    [SerializeReference, SelectableSerializeReference, Header("スキル")] private List<BaseSkillData> _skills;

    public string UnitName=> _unitName;
    public Sprite FullSprite => _fullSprite;
    public Sprite FaceSprite => _faceSprite;
    public int MaxHealth => _maxHealth;
    public int MaxMagicPoint => _maxMagicPoint;
    public int AttackPoint => _attackPoint;
    public int MagicDefensePoint => _magicDefensePoint;
    public int PhysDefensePoint => _physDefensePoint;
    public int Speed => _speed;
    public EffectResistances EffectResistances => _effectResistances;
    public List<BaseSkillData> Skills => _skills;
    public abstract UnitType UnitType { get;  }
}

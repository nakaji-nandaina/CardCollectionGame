using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Player,
    Enemy
}

public abstract class BaseUnitData : BaseScriptableObject
{
    [SerializeField, Header("���O")] private string _unitName;
    [SerializeField, Header("�S�g�摜")] private Sprite _fullSprite;
    [SerializeField, Header("��摜")] private Sprite _faceSprite;
    [SerializeField, Header("�̗�")] private int _maxHealth;
    [SerializeField, Header("����")] private int _maxMagicPoint;
    [SerializeField, Header("�U����")] private int _attackPoint;
    [SerializeField, Header("���@�h���")] private int _magicDefensePoint;
    [SerializeField, Header("�����h���")] private int _physDefensePoint;
    [SerializeReference, SelectableSerializeReference, Header("�X�L��")] private List<BaseSkillData> _skills;

    public string UnitName=> _unitName;
    public Sprite FullSprite => _fullSprite;
    public Sprite FaceSprite => _faceSprite;
    public int MaxHealth => _maxHealth;
    public int MaxMagicPoint => _maxMagicPoint;
    public int AttackPoint => _attackPoint;
    public int MagicDefensePoint => _magicDefensePoint;
    public int PhysDefensePoint => _physDefensePoint;
    public List<BaseSkillData> Skills => _skills;
    public abstract UnitType UnitType { get;  }
}

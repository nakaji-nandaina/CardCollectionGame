using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSlot
{
    public string InstanceId;
    public PlayerUnitData UnitData;
    public int Level;
    public int Exp;

    /// <summary>
    /// �U����
    /// ��b�U���� + (��b�U���͂�10% * (���x�� - 1) * 2)
    /// </summary>
    public int AttackPoint => UnitData.AttackPoint + Mathf.CeilToInt(UnitData.AttackPoint/10) * (Level - 1) * 2;

    /// <summary>
    /// �ő�̗�
    /// ��b�̗� + (��b�̗͂�10% * (���x�� - 1) * 5)
    /// </summary>
    public int MaxHp => UnitData.MaxHealth + Mathf.CeilToInt(UnitData.MaxHealth / 10) * (Level - 1) * 5;

    /// <summary>
    /// �ő喂��
    /// ��b���� + (��b���͂�10% * (���x�� - 1) * 3)
    /// </summary>
    public int MaxMp => UnitData.MaxMagicPoint + Mathf.CeilToInt(UnitData.MaxMagicPoint / 10) * (Level - 1) * 3;

    /// <summary>
    /// ���ݑ̗�
    /// </summary>
    public int CurrentHp { get; set; }

    /// <summary>
    /// ���ݖ���
    /// </summary>
    public int CurrentMp { get; set; }

    /// <summary>
    /// �����h��
    /// ��b�����h�� + (��b�����h���10% * (���x�� - 1))
    /// </summary>
    public int PhysDefensePoint => UnitData.PhysDefensePoint + Mathf.CeilToInt(UnitData.PhysDefensePoint / 10) * (Level - 1);

    /// <summary>
    /// ���@�h��
    /// ��b���@�h�� + (��b���@�h���10% * (���x�� - 1))
    /// </summary>
    public int MagicDefensePoint => UnitData.MagicDefensePoint + Mathf.CeilToInt(UnitData.MagicDefensePoint / 10) * (Level - 1);

    /// <summary>
    /// ���x
    /// </summary>
    public int Speed => UnitData.Speed;

    /// <summary>
    /// ��Ԉُ�ϐ�
    /// </summary>
    public EffectResistances EffectResistances => UnitData.EffectResistances;

    /// <summary>
    /// �X�L���f�[�^�ꗗ
    /// </summary>
    public List<BaseSkillData> Skills => UnitData.Skills;
    public UnitSlot(PlayerUnitData unitData, int level = 1, int exp = 0)
    {
        InstanceId = Guid.NewGuid().ToString();
        UnitData = unitData;
        Level = level;
        Exp = exp;
        CurrentHp = UnitData.MaxHealth + Mathf.CeilToInt(UnitData.MaxHealth / 10) * (Level - 1) * 5;
        CurrentMp = UnitData.MaxMagicPoint + Mathf.CeilToInt(UnitData.MaxMagicPoint / 10) * (Level - 1) * 3;
    }

    public UnitSlot(string instanceId,PlayerUnitData unitData, int level = 1, int exp = 0)
    {
        InstanceId = instanceId;
        UnitData = unitData;
        Level = level;
        Exp = exp;
        CurrentHp = UnitData.MaxHealth + Mathf.CeilToInt(UnitData.MaxHealth / 10) * (Level - 1) * 5;
        CurrentMp = UnitData.MaxMagicPoint + Mathf.CeilToInt(UnitData.MaxMagicPoint / 10) * (Level - 1) * 3;
    }

    public void AddExp(int amount)
    {
        if (Level >= Const.Unit.MaxLevel)
        {
            Exp = 0;
            return;
        }
        Exp += amount;
        while (Exp >= GetExpToNextLevel() && Level < Const.Unit.MaxLevel)
        {
            Exp -= GetExpToNextLevel();
            LevelUp();
        }
        if (Level >= Const.Unit.MaxLevel)
        {
            Exp = 0;
        }
    }
    public int GetExpToNextLevel()
    {
        int nextExp = Mathf.RoundToInt(Const.Unit.InitialNeedExp * Mathf.Pow(1.1f, Level));
        return nextExp;
    }

    private void LevelUp()
    {
        Level++;
        SetCurrent(MaxHp, MaxMp);
        Debug.Log($"{UnitData.UnitName} leveled up to {Level}!");
    }
    public void SetCurrent(int hp, int mp)
    {
        CurrentHp = Mathf.Clamp(hp, 0, MaxHp);
        CurrentMp = Mathf.Clamp(mp, 0, MaxMp);
    }
}

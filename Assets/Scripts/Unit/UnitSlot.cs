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
    /// UŒ‚—Í
    /// Šî‘bUŒ‚—Í + (Šî‘bUŒ‚—Í‚Ì10% * (ƒŒƒxƒ‹ - 1) * 2)
    /// </summary>
    public int AttackPoint => UnitData.AttackPoint + Mathf.CeilToInt(UnitData.AttackPoint * (Level - 1) * 2 / 10);

    /// <summary>
    /// Å‘å‘Ì—Í
    /// Šî‘b‘Ì—Í + (Šî‘b‘Ì—Í‚Ì10% * (ƒŒƒxƒ‹ - 1) * 5)
    /// </summary>
    public int MaxHp => UnitData.MaxHealth + Mathf.CeilToInt(UnitData.MaxHealth * (Level - 1) * 5 / 10);

    /// <summary>
    /// Å‘å–‚—Í
    /// Šî‘b–‚—Í + (Šî‘b–‚—Í‚Ì10% * (ƒŒƒxƒ‹ - 1) * 3)
    /// </summary>
    public int MaxMp => UnitData.MaxMagicPoint + Mathf.CeilToInt(UnitData.MaxMagicPoint * (Level - 1) * 3 / 10);

    /// <summary>
    /// Œ»İ‘Ì—Í
    /// </summary>
    public int CurrentHp { get; set; }

    /// <summary>
    /// Œ»İ–‚—Í
    /// </summary>
    public int CurrentMp { get; set; }

    /// <summary>
    /// ƒ†ƒjƒbƒg‚Ì¶‘¶ó‘Ô
    /// </summary>
    public bool IsAlive => CurrentHp > 0;

    /// <summary>
    /// •¨—–hŒä
    /// Šî‘b•¨—–hŒä + (Šî‘b•¨—–hŒä‚Ì10% * (ƒŒƒxƒ‹ - 1))
    /// </summary>
    public int PhysDefensePoint => UnitData.PhysDefensePoint + Mathf.CeilToInt(UnitData.PhysDefensePoint * (Level - 1) / 10);

    /// <summary>
    /// –‚–@–hŒä
    /// Šî‘b–‚–@–hŒä + (Šî‘b–‚–@–hŒä‚Ì10% * (ƒŒƒxƒ‹ - 1))
    /// </summary>
    public int MagicDefensePoint => UnitData.MagicDefensePoint + Mathf.CeilToInt(UnitData.MagicDefensePoint * (Level - 1) / 10);

    /// <summary>
    /// ‘¬“x
    /// </summary>
    public int Speed => UnitData.Speed;

    /// <summary>
    /// ó‘ÔˆÙí‘Ï«
    /// </summary>
    public EffectResistances EffectResistances => UnitData.EffectResistances;

    /// <summary>
    /// ƒXƒLƒ‹ƒf[ƒ^ˆê——
    /// </summary>
    public List<BaseSkillData> Skills => UnitData.Skills;
    public UnitSlot(PlayerUnitData unitData, int level = 1, int exp = 0)
    {
        InstanceId = Guid.NewGuid().ToString();
        UnitData = unitData;
        Level = level;
        Exp = exp;
        CurrentHp = MaxHp;
        CurrentMp = MaxMp;
    }

    public UnitSlot(string instanceId,PlayerUnitData unitData, int level = 1, int exp = 0)
    {
        InstanceId = instanceId;
        UnitData = unitData;
        Level = level;
        Exp = exp;
        CurrentHp = MaxHp;
        CurrentMp = MaxMp;
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

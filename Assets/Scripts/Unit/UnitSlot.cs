using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UnitSlot
{
    public string InstanceId;
    public PlayerUnitData UnitData;
    public int Level;
    public int Exp;

    /// <summary>
    /// UŒ‚—Í
    /// Šî‘bUŒ‚—Í + (Šî‘bUŒ‚—Í‚Ì10% * (ƒŒƒxƒ‹ - 1) * 2)
    /// </summary>
    public int AttackPoint => UnitData.AttackPoint + Mathf.CeilToInt(UnitData.AttackPoint/10) * (Level - 1) * 2;

    /// <summary>
    /// Å‘å‘Ì—Í
    /// Šî‘b‘Ì—Í + (Šî‘b‘Ì—Í‚Ì10% * (ƒŒƒxƒ‹ - 1) * 5)
    /// </summary>
    public int MaxHp => UnitData.MaxHealth + Mathf.CeilToInt(UnitData.MaxHealth / 10) * (Level - 1) * 5;

    /// <summary>
    /// Œ»İ‘Ì—Í
    /// </summary>
    public int CurrentHp { get; set; }

    /// <summary>
    /// •¨—–hŒä
    /// Šî‘b•¨—–hŒä + (Šî‘b•¨—–hŒä‚Ì10% * (ƒŒƒxƒ‹ - 1))
    /// </summary>
    public int PhysDefensePoint => UnitData.PhysDefensePoint + Mathf.CeilToInt(UnitData.PhysDefensePoint / 10) * (Level - 1);

    /// <summary>
    /// –‚–@–hŒä
    /// Šî‘b–‚–@–hŒä + (Šî‘b–‚–@–hŒä‚Ì10% * (ƒŒƒxƒ‹ - 1))
    /// </summary>
    public int MagicDefensePoint => UnitData.MagicDefensePoint + Mathf.CeilToInt(UnitData.MagicDefensePoint / 10) * (Level - 1);


    public UnitSlot(PlayerUnitData unitData, int level = 1, int exp = 0)
    {
        InstanceId = Guid.NewGuid().ToString();
        UnitData = unitData;
        Level = level;
        Exp = exp;
        CurrentHp = UnitData.MaxHealth + Mathf.CeilToInt(UnitData.MaxHealth / 10) * (Level - 1) * 5;
    }

    public UnitSlot(string instanceId,PlayerUnitData unitData, int level = 1, int exp = 0)
    {
        InstanceId = instanceId;
        UnitData = unitData;
        Level = level;
        Exp = exp;
        CurrentHp = UnitData.MaxHealth + Mathf.CeilToInt(UnitData.MaxHealth / 10) * (Level - 1) * 5;
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
            Level++;
            Debug.Log($"{UnitData.UnitName} leveled up to {Level}!");
        }
        if (Level >= Const.Unit.MaxLevel)
        {
            Level = Const.Unit.MaxLevel;
            Exp = 0;
        }
    }
    public int GetExpToNextLevel()
    {
        int nextExp = Mathf.RoundToInt(Const.Unit.InitialNeedExp * Mathf.Pow(1.1f, Level));
        return nextExp;
    }
}

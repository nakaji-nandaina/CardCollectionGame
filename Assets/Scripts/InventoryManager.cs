using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    private List<UnitSlot> _formedUnitList = new List<UnitSlot>();
    private List<UnitSlot> _ownedUnitList = new List<UnitSlot>();
    private List<UnitSlot> _inBattleUnitList = new List<UnitSlot>();

    public List<UnitSlot> InBattleUnitList => _inBattleUnitList;
    public List<UnitSlot> FormedUnitList => _formedUnitList;
    public List<UnitSlot> OwnedUnitList => _ownedUnitList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 編成ユニットをバトルユニットに反映
    /// </summary>
    public void SetBattleUnit()
    {
        _inBattleUnitList = new List<UnitSlot>(_formedUnitList);
    }

    /// <summary>
    /// バトルユニットの状態を編成ユニットに反映
    /// </summary>
    public void SetFormedUnit()
    {
        _formedUnitList = new List<UnitSlot>(_inBattleUnitList);
        foreach (var unit in _formedUnitList)
        {
            unit.SetCurrent(unit.MaxHp, unit.MaxMp);
        }
    }

    /// <summary>
    /// 戦闘中のユニットに経験値を分配
    /// </summary>
    /// <param name="exp"></param>
    public void AddBattleUnitExp(int exp)
    {
        // バトルユニットがいなければ終了
        if (_inBattleUnitList.Count == 0) 
            return;
        // 生存しているユニットに経験値を分配
        foreach (var unit in _inBattleUnitList)
        {
            if (!unit.IsAlive) 
                continue;
            unit.AddExp(exp);
        }
    }

    /// <summary>
    /// 編成中のユニットに経験値を分配
    /// </summary>
    /// <param name="exp"></param>
    public void AddFormedUnitExp(int exp)
    {
        if (_formedUnitList.Count == 0) 
            return;

        foreach (var unit in _formedUnitList)
        {
            unit.AddExp(exp);
        }
    }

    /// <summary>
    /// 戦闘中のユニットのHPとMPを更新
    /// </summary>
    /// <param name="currentHps"></param>
    /// <param name="currentMps"></param>
    public void UpdateBattleUnitState(List<int> currentHps, List<int> currentMps)
    {
        for (int i = 0; i < _inBattleUnitList.Count; i++)
        {
            _inBattleUnitList[i].SetCurrent(currentHps[i], currentMps[i]);
        }
    }


    public void AddFormedUnit(UnitSlot unit)
    {
        if (_formedUnitList.Count >= 5)
        {
            Debug.LogWarning("Already formed 5 units.");
            return;
        }
        if (_formedUnitList.Contains(unit))
        {
            Debug.LogWarning("Unit already formed.");
            return;
        }
        _formedUnitList.Add(unit);
    }

    public void RemoveFormedUnit(UnitSlot unit)
    {
        if (_formedUnitList.Contains(unit))
        {
            _formedUnitList.Remove(unit);
        }
        else
        {
            Debug.LogWarning("Unit not found in formed list.");
        }
    }

    public void AddOwnedUnit(UnitSlot unit)
    {
        if (_ownedUnitList.Contains(unit))
        {
            Debug.LogWarning("Unit already owned.");
            return;
        }
        _ownedUnitList.Add(unit);
    }

    public void RemoveOwnedUnit(UnitSlot unit)
    {
        if (_ownedUnitList.Contains(unit))
        {
            _ownedUnitList.Remove(unit);
        }
        else
        {
            Debug.LogWarning("Unit not found in owned list.");
        }
    }

    public void FormedToOwn(UnitSlot unit)
    {
        RemoveFormedUnit(unit);
        AddOwnedUnit(unit);
    }
    public void OwnToFormed(UnitSlot unit)
    {
        RemoveOwnedUnit(unit);
        AddFormedUnit(unit);
    }
}

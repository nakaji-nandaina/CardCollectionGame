using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Unit Data", menuName = "Unit/Enemy Unit Data")]
public class EnemyUnitData : BaseUnitData
{
    [SerializeField,Header("ŒoŒ±’l")] private int _expPoint;
    public override UnitType UnitType => UnitType.Enemy;
    public int ExpPoint => _expPoint;
}

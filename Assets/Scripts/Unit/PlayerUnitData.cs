using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerUnitData", menuName = "Unit/PlayerUnitData")]
public class PlayerUnitData : BaseUnitData
{
    public override UnitType UnitType => UnitType.Player;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<UnitProgress> FormedUnits;
    public List<UnitProgress> InventoryUnits;
    public List<string> InventoryEquipments;
    public int Gold;
    public float SEVolume;
    public float BGMVolume;

    public SaveData()
    {
        FormedUnits = new List<UnitProgress>();
        InventoryUnits = new List<UnitProgress>();
        InventoryEquipments = new List<string>();
        Gold = 0;
        SEVolume = 1f;
        BGMVolume = 1f;
    }

}

[Serializable]
public class UnitProgress
{
    public string InstanceId;
    public string UnitId;
    public int Level = 1;
    public int Exp = 0;
}

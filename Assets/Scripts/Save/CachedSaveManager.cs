using UnityEngine;

public static class CachedSaveManager
{
    private static SaveData _saveData;
    public static SaveData CachedSaveData
    {
        get
        {
            if (_saveData == null)
            {
                Load();
                return _saveData;
            }
            else
            {
                return _saveData;
            }
        }
        private set
        {
            _saveData = value;
        }
    }

    public static void Load()
    {
        try
        {
            CachedSaveData = SaveIO.Load();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
            CachedSaveData = new SaveData();
        }
    }

    public static void Save()
    {
        SaveIO.Save(CachedSaveData);
    }
}

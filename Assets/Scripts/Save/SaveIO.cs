using UnityEngine;

public class SaveIO
{
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(Const.Save.SaveId, json);
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        //セーブデータが存在しない場合は例外を投げる
        if (!PlayerPrefs.HasKey(Const.Save.SaveId))
        {
            throw new System.Exception("No Save Data");
            
        }
        string json = PlayerPrefs.GetString(Const.Save.SaveId);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        //dataが壊れていないかチェック
        if (data == null)
        {
            throw new System.Exception("Save Data Corrupted");
        }
        return data;
    }
}

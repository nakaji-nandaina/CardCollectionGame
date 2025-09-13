using System;
using UnityEditor;
using UnityEngine;
public class ScriptableObjectIdAttribute : PropertyAttribute
{
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true); GUI.enabled = true;
    }
} 
#endif 

public class BaseScriptableObject : ScriptableObject 
{ 
    [ScriptableObjectId] public string Id;
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Editor上のアセット or サブアセットなら GUID と LocalID を採用
        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out string guid, out long localId))
        {
            string composed = $"{guid}:{localId}";
            if (Id != composed)
            {
                Id = composed;
                EditorUtility.SetDirty(this);
            }
        }
        else
        {
            // まだアセット化されていない等で失敗した場合、空なら一時IDを付与
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.NewGuid().ToString();
                EditorUtility.SetDirty(this);
            }
        }
    }
#endif
}
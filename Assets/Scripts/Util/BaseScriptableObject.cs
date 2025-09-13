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
        // Editor��̃A�Z�b�g or �T�u�A�Z�b�g�Ȃ� GUID �� LocalID ���̗p
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
            // �܂��A�Z�b�g������Ă��Ȃ����Ŏ��s�����ꍇ�A��Ȃ�ꎞID��t�^
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.NewGuid().ToString();
                EditorUtility.SetDirty(this);
            }
        }
    }
#endif
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogView : MonoBehaviour
{
    [SerializeField,Header("テキストプレハブ")] private Text _textPrefab;
    [SerializeField,Header("ログ表示スクロールビュー")] private ScrollRect _logScrollView;
    public void SetUp()
    {
        //スクロールビューの初期化
        foreach(Transform child in _logScrollView.content)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddLog(string log)
    {
        Text newLog = Instantiate(_textPrefab, _logScrollView.content);
        newLog.text = log;
        _logScrollView.verticalNormalizedPosition = 0f;
    }
}

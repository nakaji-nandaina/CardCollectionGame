using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogView : MonoBehaviour
{
    [SerializeField,Header("�e�L�X�g�v���n�u")] private Text _textPrefab;
    [SerializeField,Header("���O�\���X�N���[���r���[")] private ScrollRect _logScrollView;
    public void SetUp()
    {
        //�X�N���[���r���[�̏�����
        foreach(Transform child in _logScrollView.content)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddLog(string log)
    {
        Text newLog = Instantiate(_textPrefab, _logScrollView.content);
        newLog.text = log;
        StartCoroutine(ScrollToBottomNextFrame());
    }
    IEnumerator ScrollToBottomNextFrame()
    {
        yield return null; // ���C�A�E�g�X�V��҂�
        _logScrollView.verticalNormalizedPosition = 0f; // ���[
    }
}

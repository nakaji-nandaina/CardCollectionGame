using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearPanelView : PanelViewBase
{
    [SerializeField, Header("�A�C�e���\���G���A")] RectTransform _itemArea;
    [SerializeField, Header("�p�[�e�B�\���G���A")] RectTransform _partyArea;
    [SerializeField, Header("�L�^�\���G���A")] RectTransform _recordArea;
    [SerializeField, Header("�A�C�e���\���̐e�I�u�W�F�N�g")] RectTransform _itemScroll;
    [SerializeField, Header("���j�b�g�\���̐e�I�u�W�F�N�g")] RectTransform _partyParent;
    [SerializeField, Header("�A�C�e���\���v���n�u")] GameObject _itemPrefab;
    [SerializeField, Header("���j�b�g�\���v���n�u")] GameObject _unitPrefab;
    [SerializeField, Header("�K�w�\���e�L�X�g")] Text _floorText;
    [SerializeField, Header("�N���A�^�C���\���e�L�X�g")] Text _clearTimeText;
    [SerializeField, Header("�Ē���{�^��")] Button _retryButton;
    [SerializeField, Header("���_�֖߂�{�^��")] Button _toTownButton;

    public void SetUp(Action OnRetryButtonPressed, Action OnToTownButtonPressed)
    {
        _retryButton.onClick.RemoveAllListeners();
        _retryButton.onClick.AddListener(() => OnRetryButtonPressed?.Invoke());
        
        _toTownButton.onClick.RemoveAllListeners();
        _toTownButton.onClick.AddListener(() => OnToTownButtonPressed?.Invoke());

        for(int i = 0; i < _partyParent.childCount; i++)
            Destroy(_partyParent.GetChild(i).gameObject);

        for(int i = 0; i < _itemScroll.childCount; i++)
            Destroy(_itemScroll.GetChild(i).gameObject);

        foreach(var unit in InventoryManager.Instance.FormedUnitList)
        {
            var unitPrefab = Instantiate(_unitPrefab, _partyParent);
        }
    }
}

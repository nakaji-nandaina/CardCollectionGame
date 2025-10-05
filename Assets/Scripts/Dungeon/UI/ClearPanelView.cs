using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearPanelView : PanelViewBase
{
    [SerializeField, Header("アイテム表示エリア")] RectTransform _itemArea;
    [SerializeField, Header("パーティ表示エリア")] RectTransform _partyArea;
    [SerializeField, Header("記録表示エリア")] RectTransform _recordArea;
    [SerializeField, Header("アイテム表示の親オブジェクト")] RectTransform _itemScroll;
    [SerializeField, Header("ユニット表示の親オブジェクト")] RectTransform _partyParent;
    [SerializeField, Header("アイテム表示プレハブ")] GameObject _itemPrefab;
    [SerializeField, Header("ユニット表示プレハブ")] GameObject _unitPrefab;
    [SerializeField, Header("階層表示テキスト")] Text _floorText;
    [SerializeField, Header("クリアタイム表示テキスト")] Text _clearTimeText;
    [SerializeField, Header("再挑戦ボタン")] Button _retryButton;
    [SerializeField, Header("拠点へ戻るボタン")] Button _toTownButton;

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

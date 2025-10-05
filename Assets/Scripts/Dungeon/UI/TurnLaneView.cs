using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class TurnLaneView : MonoBehaviour
{
    [SerializeField, Header("ターンアイコン")] private List<TurnIconView> _turnIconViews = new List<TurnIconView>();

    // 調整可能なアニメ設定
    [SerializeField, Header("アニメ時間")] private float _moveDuration = 0.22f;
    [SerializeField, Header("割り込みスライド時間")] private float _cutInDuration = 0.28f;
    [SerializeField, Header("死亡スライド時間")] private float _removeDuration = 0.25f;
    [SerializeField, Header("割り込み開始オフセット（Y）")] private float _cutInOffsetY = 120f;
    [SerializeField, Header("死亡スライドオフセット（Y）")] private float _removeOffsetY = 80f;

    // 初期スロット位置（Inspector の配置を基準にキャプチャ）
    private List<Vector2> _slotPositions = new List<Vector2>();

    // 現在表示中の InstanceId リスト（順序）
    private List<string> _currentIds = new List<string>();

    private void Awake()
    {
        // スロットの基準位置をキャプチャ
        _slotPositions.Clear();
        foreach (var v in _turnIconViews)
        {
            var rt = v.GetRectTransform();
            _slotPositions.Add(rt != null ? rt.anchoredPosition : Vector2.zero);
        }

        // 初期はすべて非表示にしておく
        foreach (var v in _turnIconViews) v.gameObject.SetActive(false);
    }

    // アニメ付き更新（OnTurnStarted から呼ぶ）
    public void AnimateTurnOrder(List<ActionNode> turnOrder)
    {
        // 表示する InstanceIds（スロット数に合わせる）
        List<string> newIds = turnOrder.Select(n => n.InstanceId).ToList();
        if (newIds.Count > _turnIconViews.Count)
            newIds = newIds.Take(_turnIconViews.Count).ToList();

        // 現在のビューを id -> view で参照できるようにする
        Dictionary<string, TurnIconView> viewById = new Dictionary<string, TurnIconView>();
        foreach (TurnIconView v in _turnIconViews)
        {
            // InstanceId が null/空 のビューはマップに含めない（存在しないキー参照を防ぐ）
            if (!string.IsNullOrEmpty(v.InstanceId))
                viewById[v.InstanceId] = v;
        }


        // 死亡したものはアイコンに委譲して複製アニメを実行
        List<string> toRemove = _currentIds.Where(id => !newIds.Contains(id)).ToList();
        foreach (var id in toRemove)
        {
            if (viewById.TryGetValue(id, out var turnIconView))
            {
                turnIconView.PlayRemoveAsClone(_removeDuration, _removeOffsetY);
                viewById.Remove(id);
            }
        }

        // 各 newId をスロットに割り当て、アイコン API を呼ぶ
        for (int idx = 0; idx < _turnIconViews.Count; idx++)
        {
            if (idx >= newIds.Count)
            {
                _turnIconViews[idx].Clear();
                continue;
            }
            string id = newIds[idx];
            // 目標位置
            Vector2 targetPos = _slotPositions[idx];
            // 対応する BattleUnit
            ActionNode node = turnOrder.FirstOrDefault(n => n.InstanceId == id);

            if (viewById.TryGetValue(id, out var existingView))
            {
                // 既存割り当て：通常移動 or 割り込みアニメ
                int prevIndex = _currentIds.IndexOf(id);
                // あたらしい位置が前-1より前に来る場合は割り込みとみなす
                int newIndex = idx;
                bool cutIn = (prevIndex - 1 > newIndex);

                if (cutIn)
                {
                    existingView.PlayCutIn(targetPos, _cutInDuration, _cutInOffsetY);
                }
                else
                {
                    existingView.MoveTo(targetPos, _moveDuration);
                }
                existingView.ScaleUp(newIndex == 0);
            }
            else
            {
                // 新規割り当て：空きスロットを探す（なければ強制上書き）
                var freeView = _turnIconViews.FirstOrDefault(v => string.IsNullOrEmpty(v.InstanceId));
                if (freeView == null) freeView = _turnIconViews[idx];
                // 割り当てて割り込みアニメ
                freeView.SetUp(node);
                freeView.PlayCutIn(targetPos, _cutInDuration, _cutInOffsetY);

                freeView.ScaleUp(false);
            }
        }

        // 3) currentIds 更新
        _currentIds = new List<string>(newIds);
    }

    public void SetUp(List<ActionNode> turnOrder)
    {
        var ids = turnOrder.Select(n => n.InstanceId).ToList();
        for (int i = 0; i < _turnIconViews.Count; i++)
        {
            if (i < ids.Count)
            {
                var node = turnOrder.FirstOrDefault(n => n.InstanceId == ids[i]);
                var v = _turnIconViews[i];
                v.SetUp(node);
                var rt = v.GetRectTransform();
                rt.anchoredPosition = _slotPositions[i];
                v.GetCanvasGroup().alpha = 1f;
                v.gameObject.SetActive(true);
            }
            else
            {
                _turnIconViews[i].Clear();
            }
        }
        _currentIds = ids;
    }
}
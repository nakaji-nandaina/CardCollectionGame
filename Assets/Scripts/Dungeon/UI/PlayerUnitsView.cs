using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class PlayerUnitsView : MonoBehaviour
{
    [SerializeField,Header("カード")] private List<BattleCardView> _playerCardPrefabList = new List<BattleCardView>();
    private readonly Dictionary<string, BattleCardView> _views = new Dictionary<string, BattleCardView>();

    public void SetUp(List<UnitSlot> unitSlots)
    {
        _views.Clear();
        int count = Mathf.Min(unitSlots.Count, _playerCardPrefabList.Count);
        for (int i = 0; i < count; i++)
        {
            var view = _playerCardPrefabList[i];
            view.SetUp(unitSlots[i]);
            _views[unitSlots[i].InstanceId] = view;
        }

        // 不要ビューは非表示にする
        for (int i = unitSlots.Count; i < _playerCardPrefabList.Count; i++)
        {
            var v = _playerCardPrefabList[i];
            if (v != null) v.gameObject.SetActive(false);
        }
    }
    // バトル準備時に BattleUnit の現在値を一括で反映する（InstanceIdでマッチ）
    public void SetUp(List<BattleController.BattleUnit> players)
    {
        foreach (var p in players)
        {
            if (string.IsNullOrEmpty(p.InstanceId)) continue;
            if (_views.TryGetValue(p.InstanceId, out var view))
            {
                view.UpdateCurrent(p.CurrentHp, p.CurrentMp);
            }
            else
            {
                // 保険: Inspector でセットしたビュー群に InstanceId を持たせているか確認してマップを補完
                var match = _playerCardPrefabList.FirstOrDefault(v => v != null && v.InstanceId == p.InstanceId);
                if (match != null)
                {
                    _views[p.InstanceId] = match;
                    match.UpdateCurrent(p.CurrentHp, p.CurrentMp);
                }
            }
        }
    }

    public void UpdateCurrent(string instanceId, int currentHp, int currentMp)
    {
        if (string.IsNullOrEmpty(instanceId)) return;
        if (_views.TryGetValue(instanceId, out var view))
        {
            view.UpdateCurrent(currentHp, currentMp);
        }
    }
    public void Damaged(string instancedId)
    {
        // ダメージエフェクト再生
        if (!_views.ContainsKey(instancedId))
        {
            return;
        }
        Transform t = _views[instancedId].transform;
        RectTransform rt = t as RectTransform;
        rt.DOKill(true); // 既存のアニメを停止
        float offsetX = 10f;
        float duration = 0.05f; // 左右1往復の片道時間

        Vector2 start = rt.anchoredPosition;
        // シーケンスで左右に振動
        Sequence seq = DOTween.Sequence();
        // 左へ
        seq.Append(rt.DOAnchorPos(start + new Vector2(-offsetX, 0), duration));
        // 右へ
        seq.Append(rt.DOAnchorPos(start + new Vector2(offsetX, 0), duration));
        // 戻る
        seq.Append(rt.DOAnchorPos(start, duration));
        // 繰り返し回数を設定（ここでは2回往復）
        seq.SetLoops(2, LoopType.Yoyo);
        // Easeで揺れっぽさを演出
        seq.SetEase(Ease.InOutQuad);
    }
    public void Attack(string instancedId)
    {
        if (!_views.ContainsKey(instancedId))
        {
            return;
        }
        Transform t = _views[instancedId].transform;

        // RectTransform を期待する場合は RectTransform を使う
        RectTransform rt = t as RectTransform;

        rt.DOKill(true); // 既存のアニメを停止
        // 現在のローカルY を基準に上に跳ねる攻撃アニメ
        float offsetY = 20f;
        float upDuration = 0.12f;
        Vector3 start= rt.anchoredPosition;
        Vector3 target = start + new Vector3(0, offsetY, 0);

        rt.DOAnchorPos(target, upDuration).SetEase(Ease.InSine).OnComplete(() =>
        {
            rt.DOAnchorPos(start, upDuration).SetEase(Ease.InSine);
        });
    }
}

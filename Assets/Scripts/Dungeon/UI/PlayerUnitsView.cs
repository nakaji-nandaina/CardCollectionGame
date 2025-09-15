using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUnitsView : MonoBehaviour
{
    [SerializeField,Header("カード")] private List<BattleCardView> _playerCardPrefabList = new List<BattleCardView>();
    private readonly Dictionary<string, BattleCardView> _views = new Dictionary<string, BattleCardView>();

    public void SetUp(List<UnitSlot> unitSlots)
    {
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
        // ダメージエフェクト再生(DOTween導入後)
    }
    public void Attack(string instancedId)
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUnitsView : MonoBehaviour
{
    [SerializeField,Header("�J�[�h")] private List<BattleCardView> _playerCardPrefabList = new List<BattleCardView>();
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

        // �s�v�r���[�͔�\���ɂ���
        for (int i = unitSlots.Count; i < _playerCardPrefabList.Count; i++)
        {
            var v = _playerCardPrefabList[i];
            if (v != null) v.gameObject.SetActive(false);
        }
    }
    // �o�g���������� BattleUnit �̌��ݒl���ꊇ�Ŕ��f����iInstanceId�Ń}�b�`�j
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
                // �ی�: Inspector �ŃZ�b�g�����r���[�Q�� InstanceId ���������Ă��邩�m�F���ă}�b�v��⊮
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
        // �_���[�W�G�t�F�N�g�Đ�(DOTween������)
    }
    public void Attack(string instancedId)
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class PlayerUnitsView : MonoBehaviour
{
    [SerializeField,Header("�J�[�h")] private List<BattleCardView> _playerCardPrefabList = new List<BattleCardView>();
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
        // �_���[�W�G�t�F�N�g�Đ�
        if (!_views.ContainsKey(instancedId))
        {
            return;
        }
        Transform t = _views[instancedId].transform;
        RectTransform rt = t as RectTransform;
        rt.DOKill(true); // �����̃A�j�����~
        float offsetX = 10f;
        float duration = 0.05f; // ���E1�����̕Г�����

        Vector2 start = rt.anchoredPosition;
        // �V�[�P���X�ō��E�ɐU��
        Sequence seq = DOTween.Sequence();
        // ����
        seq.Append(rt.DOAnchorPos(start + new Vector2(-offsetX, 0), duration));
        // �E��
        seq.Append(rt.DOAnchorPos(start + new Vector2(offsetX, 0), duration));
        // �߂�
        seq.Append(rt.DOAnchorPos(start, duration));
        // �J��Ԃ��񐔂�ݒ�i�����ł�2�񉝕��j
        seq.SetLoops(2, LoopType.Yoyo);
        // Ease�ŗh����ۂ������o
        seq.SetEase(Ease.InOutQuad);
    }
    public void Attack(string instancedId)
    {
        if (!_views.ContainsKey(instancedId))
        {
            return;
        }
        Transform t = _views[instancedId].transform;

        // RectTransform �����҂���ꍇ�� RectTransform ���g��
        RectTransform rt = t as RectTransform;

        rt.DOKill(true); // �����̃A�j�����~
        // ���݂̃��[�J��Y ����ɏ�ɒ��˂�U���A�j��
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class TurnLaneView : MonoBehaviour
{
    [SerializeField, Header("�^�[���A�C�R��")] private List<TurnIconView> _turnIconViews = new List<TurnIconView>();

    // �����\�ȃA�j���ݒ�
    [SerializeField, Header("�A�j������")] private float _moveDuration = 0.22f;
    [SerializeField, Header("���荞�݃X���C�h����")] private float _cutInDuration = 0.28f;
    [SerializeField, Header("���S�X���C�h����")] private float _removeDuration = 0.25f;
    [SerializeField, Header("���荞�݊J�n�I�t�Z�b�g�iY�j")] private float _cutInOffsetY = 120f;
    [SerializeField, Header("���S�X���C�h�I�t�Z�b�g�iY�j")] private float _removeOffsetY = 80f;

    // �����X���b�g�ʒu�iInspector �̔z�u����ɃL���v�`���j
    private List<Vector2> _slotPositions = new List<Vector2>();

    // ���ݕ\������ InstanceId ���X�g�i�����j
    private List<string> _currentIds = new List<string>();

    private void Awake()
    {
        // �X���b�g�̊�ʒu���L���v�`��
        _slotPositions.Clear();
        foreach (var v in _turnIconViews)
        {
            var rt = v.GetRectTransform();
            _slotPositions.Add(rt != null ? rt.anchoredPosition : Vector2.zero);
        }

        // �����͂��ׂĔ�\���ɂ��Ă���
        foreach (var v in _turnIconViews) v.gameObject.SetActive(false);
    }

    // �A�j���t���X�V�iOnTurnStarted ����Ăԁj
    public void AnimateTurnOrder(List<ActionNode> turnOrder)
    {
        // �\������ InstanceIds�i�X���b�g���ɍ��킹��j
        List<string> newIds = turnOrder.Select(n => n.InstanceId).ToList();
        if (newIds.Count > _turnIconViews.Count)
            newIds = newIds.Take(_turnIconViews.Count).ToList();

        // ���݂̃r���[�� id -> view �ŎQ�Ƃł���悤�ɂ���
        Dictionary<string, TurnIconView> viewById = new Dictionary<string, TurnIconView>();
        foreach (TurnIconView v in _turnIconViews)
        {
            // InstanceId �� null/�� �̃r���[�̓}�b�v�Ɋ܂߂Ȃ��i���݂��Ȃ��L�[�Q�Ƃ�h���j
            if (!string.IsNullOrEmpty(v.InstanceId))
                viewById[v.InstanceId] = v;
        }


        // ���S�������̂̓A�C�R���ɈϏ����ĕ����A�j�������s
        List<string> toRemove = _currentIds.Where(id => !newIds.Contains(id)).ToList();
        foreach (var id in toRemove)
        {
            if (viewById.TryGetValue(id, out var turnIconView))
            {
                turnIconView.PlayRemoveAsClone(_removeDuration, _removeOffsetY);
                viewById.Remove(id);
            }
        }

        // �e newId ���X���b�g�Ɋ��蓖�āA�A�C�R�� API ���Ă�
        for (int idx = 0; idx < _turnIconViews.Count; idx++)
        {
            if (idx >= newIds.Count)
            {
                _turnIconViews[idx].Clear();
                continue;
            }
            string id = newIds[idx];
            // �ڕW�ʒu
            Vector2 targetPos = _slotPositions[idx];
            // �Ή����� BattleUnit
            ActionNode node = turnOrder.FirstOrDefault(n => n.InstanceId == id);

            if (viewById.TryGetValue(id, out var existingView))
            {
                // �������蓖�āF�ʏ�ړ� or ���荞�݃A�j��
                int prevIndex = _currentIds.IndexOf(id);
                // �����炵���ʒu���O-1���O�ɗ���ꍇ�͊��荞�݂Ƃ݂Ȃ�
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
                // �V�K���蓖�āF�󂫃X���b�g��T���i�Ȃ���΋����㏑���j
                var freeView = _turnIconViews.FirstOrDefault(v => string.IsNullOrEmpty(v.InstanceId));
                if (freeView == null) freeView = _turnIconViews[idx];
                // ���蓖�ĂĊ��荞�݃A�j��
                freeView.SetUp(node);
                freeView.PlayCutIn(targetPos, _cutInDuration, _cutInOffsetY);

                freeView.ScaleUp(false);
            }
        }

        // 3) currentIds �X�V
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
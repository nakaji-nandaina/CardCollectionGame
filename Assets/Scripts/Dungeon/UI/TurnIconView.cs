using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TurnIconView : MonoBehaviour
{
    [SerializeField, Header("���j�b�g�A�C�R��")] private Image _iconImage;
    [SerializeField, Header("�A�C�R���w�i")] private Image _iconBackground;
    [SerializeField, Header("�A�C�R���t���[��")] private Image _iconFrame;
    //#870404
    private readonly Color _enemyColor = new Color(0.529f, 0.016f, 0.016f);
    //#55A4A6
    private readonly Color _playerColor = new Color(0.333f, 0.643f, 0.651f);
    //#F8F074
    private readonly Color _activeFrameColor = new Color(0.973f, 0.941f, 0.455f);

    // ���݊��蓖�Ă��Ă��郆�j�b�g�� InstanceId
    public string InstanceId { get; private set; }

    // �������inull �ŃN���A�j
    public void SetUp(ActionNode node)
    {
        BattleUnit battleUnit = node.Unit;

        InstanceId = node.InstanceId;

        BaseUnitData unitData = battleUnit.Data;
        if (unitData != null)
        {
            _iconImage.sprite = unitData.FaceSprite;
            if (unitData.UnitType == UnitType.Enemy)
            {
                _iconBackground.color = _enemyColor;
            }
            else
            {
                _iconBackground.color = _playerColor;
            }
        }

        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
        cg.DOKill(true);
        cg.alpha = 0f;
        cg.DOFade(1f, 0.28f);

        gameObject.SetActive(true);
    }

    // RectTransform �擾
    public RectTransform GetRectTransform()
    {
        return transform as RectTransform;
    }

    // CanvasGroup �擾�i�Ȃ���Βǉ��j
    public CanvasGroup GetCanvasGroup()
    {
        var cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    // �N���A�i��\���ɂ��� InstanceId ���N���A�j
    public void Clear()
    {
        InstanceId = null;
        var rt = GetRectTransform();
        var cg = GetCanvasGroup();
        if (rt != null) rt.DOKill(true);
        if (cg != null) cg.DOKill(true);
        gameObject.SetActive(false);
    }

    // �w��ʒu�֊��炩�Ɉړ��i�ʏ�ړ��p�j
    public void MoveTo(Vector2 targetPos, float duration)
    {
        var rt = GetRectTransform();
        var cg = GetCanvasGroup();
        if (rt == null) return;
        rt.DOKill(true);
        cg.DOKill(true);
        gameObject.SetActive(true);
        rt.DOAnchorPos(targetPos, duration).SetEase(Ease.OutCubic);
        cg.DOFade(1f, Mathf.Max(0.01f, duration * 0.6f));
    }

    // ���荞�݁icut-in�j�F�ォ��~��Ă��� target �ɒ��n����\��
    public void PlayCutIn(Vector2 targetPos, float duration, float startOffsetY)
    {
        var rt = GetRectTransform();
        var cg = GetCanvasGroup();
        if (rt == null) return;
        rt.DOKill(true);
        cg.DOKill(true);
        rt.anchoredPosition = targetPos + new Vector2(0, startOffsetY);
        cg.alpha = 0f;
        gameObject.SetActive(true);
        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOAnchorPos(targetPos, duration).SetEase(Ease.OutCubic));
        seq.Join(cg.DOFade(1f, Mathf.Max(0.01f, duration * 0.6f)));
    }

    // ���S�A�j���F���݂̌����ڂ𕡐����ď㏸�t�F�[�h�A�E�g������B�I���W�i���͑����N���A���čė��p�\�ɂ���B
    public void PlayRemoveAsClone(float duration, float offsetY)
    {
        var originalRt = GetRectTransform();
        var originalCg = GetCanvasGroup();

        // ���������i�e�͓����j
        GameObject clone = Instantiate(gameObject, transform.parent);
        var cloneRt = clone.GetComponent<RectTransform>();
        var cloneCg = clone.GetComponent<CanvasGroup>();
        if (cloneCg == null) cloneCg = clone.AddComponent<CanvasGroup>();

        if (originalRt != null && cloneRt != null)
        {
            cloneRt.anchoredPosition = originalRt.anchoredPosition;
            cloneRt.localScale = originalRt.localScale;
            cloneRt.localRotation = originalRt.localRotation;
        }

        // start alpha: �����قړ����Ȃ猩���Ȃ��Ȃ�̂�h���i�Œ�����ɂ���j
        float startAlpha = 1f;
        if (originalCg != null) startAlpha = originalCg.alpha;
        if (startAlpha < 0.05f) startAlpha = 1f;
        cloneCg.alpha = startAlpha;

        // �I���W�i���͑����N���A���đ��ōė��p�\�ɂ���
        if (originalRt != null) originalRt.DOKill(true);
        if (originalCg != null) originalCg.DOKill(true);
        Clear();

        // ��������փX���C�h���t�F�[�h�A�E�g���Ĕj��
        Sequence seq = DOTween.Sequence();
        seq.Append(cloneCg.DOFade(0f, duration));
        if (cloneRt != null)
        {
            seq.Join(cloneRt.DOAnchorPos(cloneRt.anchoredPosition + new Vector2(0, offsetY), duration).SetEase(Ease.InBack));
        }
        seq.OnComplete(() => Destroy(clone));
    }

    /// <summary>
    /// �����ڂ����������� targetPos �ֈړ�������B�I���W�i���͑��� Clear ����X���b�g�͑����蓖�ĉ\�ɂȂ�B
    /// cutIn: true �̏ꍇ�� startOffsetY �𗘗p���ĊJ�n�ʒu����i+Y�j�ɂ��炵�t�F�[�h�C�����Ē��n�B
    /// fadeOutOnMove: true �̎��͈ړ����Ƀt�F�[�h�A�E�g���Ĕj������i�ړ��ŏ�����\���j�B
    /// </summary>
    public void PlayMoveAsCloneTo(Vector2 targetPos, float duration, bool cutIn = false, float startOffsetY = 0f, bool fadeOutOnMove = true)
    {
        var originalRt = GetRectTransform();
        var originalCg = GetCanvasGroup();

        if (originalRt == null)
        {
            if (originalCg != null) originalCg.DOKill(true);
            Clear();
            return;
        }

        // �����쐬
        GameObject clone = Instantiate(gameObject, transform.parent);
        var cloneRt = clone.GetComponent<RectTransform>();
        var cloneCg = clone.GetComponent<CanvasGroup>();
        if (cloneCg == null) cloneCg = clone.AddComponent<CanvasGroup>();

        // ������ԃZ�b�g
        cloneRt.anchoredPosition = originalRt.anchoredPosition + (cutIn ? new Vector2(0, startOffsetY) : Vector2.zero);
        cloneRt.localScale = originalRt.localScale;
        cloneRt.localRotation = originalRt.localRotation;

        // �����͊m���ɉ��ŊJ�n����i�����t�F�[�h���� alpha ����������Ԃ��R�s�[���Ȃ��j
        cloneCg.alpha = 1f;

        // �I���W�i���͑����N���A���čė��p�\�ɂ���
        originalRt.DOKill(true);
        if (originalCg != null) originalCg.DOKill(true);
        Clear();

        // �A�j�����s
        Sequence seq = DOTween.Sequence();

        seq.Append(cloneRt.DOAnchorPos(targetPos, duration).SetEase(Ease.OutCubic));
        if (fadeOutOnMove)
            seq.Join(cloneCg.DOFade(0f, duration * 0.9f));
        else
            seq.Join(cloneCg.DOFade(1f, Mathf.Max(0.01f, duration * 0.6f)));

        seq.OnComplete(() => Destroy(clone));
    }

    public void ScaleUp(bool up)
    {
        gameObject.transform.localScale = up ? new Vector3(1.2f, 1.2f, 1) : Vector3.one;
        _iconFrame.color = up ? _activeFrameColor : Color.white;
    }
}
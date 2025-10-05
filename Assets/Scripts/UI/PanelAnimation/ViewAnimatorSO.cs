using UnityEngine;
using DG.Tweening;

/// <summary>
/// UI�r���[�̃A�j���[�V�������`���钊�ۃN���X
/// ScriptableObject�����邱�ƂŁA�v���W�F�N�g�S�̂ōė��p�\�ɂ���
/// </summary>
public abstract class ViewAnimatorSO : ScriptableObject
{
    /// <summary>�\�����̃A�j���[�V����</summary>
    public abstract Tween PlayIn(RectTransform rt, CanvasGroup cg);

    /// <summary>��\�����̃A�j���[�V����</summary>
    public abstract Tween PlayOut(RectTransform rt, CanvasGroup cg);
}
using UnityEngine;
using DG.Tweening;

/// <summary>
/// UIビューのアニメーションを定義する抽象クラス
/// ScriptableObject化することで、プロジェクト全体で再利用可能にする
/// </summary>
public abstract class ViewAnimatorSO : ScriptableObject
{
    /// <summary>表示時のアニメーション</summary>
    public abstract Tween PlayIn(RectTransform rt, CanvasGroup cg);

    /// <summary>非表示時のアニメーション</summary>
    public abstract Tween PlayOut(RectTransform rt, CanvasGroup cg);
}
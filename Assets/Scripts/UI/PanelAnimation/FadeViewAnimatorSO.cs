using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "UI/ViewAnimators/Fade")]
public class FadeViewAnimatorSO : ViewAnimatorSO
{
    [SerializeField] float inDuration = 0.18f;
    [SerializeField] float outDuration = 0.15f;
    [SerializeField] Ease ease = Ease.Linear;

    public override Tween PlayIn(RectTransform rt, CanvasGroup cg)
    {
        cg.alpha = 0f;
        return cg.DOFade(1f, inDuration).SetEase(ease);
    }

    public override Tween PlayOut(RectTransform rt, CanvasGroup cg)
    {
        return cg.DOFade(0f, outDuration).SetEase(ease);
    }
}

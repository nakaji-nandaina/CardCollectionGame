using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "UI/ViewAnimators/Slide")]
public class SlideViewAnimatorSO : ViewAnimatorSO
{
    [SerializeField] float inDuration = 0.25f;
    [SerializeField] float outDuration = 0.20f;
    [SerializeField] float offsetY = 120f;       // スライド開始オフセット
    [SerializeField] Ease inEase = Ease.OutCubic;
    [SerializeField] Ease outEase = Ease.InCubic;

    public override Tween PlayIn(RectTransform rt, CanvasGroup cg)
    {
        var start = rt.anchoredPosition;
        rt.anchoredPosition = start + new Vector2(0, -offsetY);
        return DOTween.Sequence().Join(rt.DOAnchorPos(start, inDuration).SetEase(inEase));
    }

    public override Tween PlayOut(RectTransform rt, CanvasGroup cg)
    {
        var start = rt.anchoredPosition;
        return DOTween.Sequence()
            .Join(rt.DOAnchorPos(start + new Vector2(0, -offsetY), outDuration).SetEase(outEase));
    }
}

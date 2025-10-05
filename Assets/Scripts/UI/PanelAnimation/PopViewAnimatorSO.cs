using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "UI/ViewAnimators/Pop")]
public class PopViewAnimatorSO : ViewAnimatorSO
{
    [SerializeField] float inDuration = 0.28f;   // 表示時間
    [SerializeField] float outDuration = 0.22f;  // 閉じる時間
    [SerializeField] Ease inEase = Ease.OutBack; 
    [SerializeField] Ease outEase = Ease.InQuad; 
    [SerializeField, Range(0f, 1f)] float startScale = 0.8f; 

    public override Tween PlayIn(RectTransform rt, CanvasGroup cg)
    {
        rt.localScale = Vector3.one * startScale;
        
        // スケールとフェードを同時に実行
        return DOTween.Sequence().Join(rt.DOScale(1f, inDuration).SetEase(inEase));
    }

    public override Tween PlayOut(RectTransform rt, CanvasGroup cg)
    {
        // 縮小しながらフェードアウト
        return DOTween.Sequence().Join(rt.DOScale(startScale, outDuration).SetEase(outEase));
    }
}

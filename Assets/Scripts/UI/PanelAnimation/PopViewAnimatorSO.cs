using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "UI/ViewAnimators/Pop")]
public class PopViewAnimatorSO : ViewAnimatorSO
{
    [SerializeField] float inDuration = 0.28f;   // �\������
    [SerializeField] float outDuration = 0.22f;  // ���鎞��
    [SerializeField] Ease inEase = Ease.OutBack; 
    [SerializeField] Ease outEase = Ease.InQuad; 
    [SerializeField, Range(0f, 1f)] float startScale = 0.8f; 

    public override Tween PlayIn(RectTransform rt, CanvasGroup cg)
    {
        rt.localScale = Vector3.one * startScale;
        
        // �X�P�[���ƃt�F�[�h�𓯎��Ɏ��s
        return DOTween.Sequence().Join(rt.DOScale(1f, inDuration).SetEase(inEase));
    }

    public override Tween PlayOut(RectTransform rt, CanvasGroup cg)
    {
        // �k�����Ȃ���t�F�[�h�A�E�g
        return DOTween.Sequence().Join(rt.DOScale(startScale, outDuration).SetEase(outEase));
    }
}

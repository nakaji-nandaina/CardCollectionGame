using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlashUIfx : BaseUIfx
{
    RectMask2D _mask;
    [SerializeField] float _duration = 0.5f;
    public override void PlayAnimation()
    {
        _mask = GetComponent<RectMask2D>();
        float height = _mask.rectTransform.rect.height * _mask.rectTransform.localScale.y;
        //マスクの初期状態を設定
        _mask.padding = new Vector4(0, height, 0, 0);
        //マスクのpaddingをアニメーションさせる
        DOTween.Sequence()
            .Append(DOTween.To(() => _mask.padding, x => _mask.padding = x, new Vector4(0, 0, 0, 0), _duration / 2))
            .Append(DOTween.To(() => _mask.padding, x => _mask.padding = x, new Vector4(0, 0, 0, height), _duration / 2))
            .SetEase(Ease.Linear)
            .OnComplete(()=>Destroy(this.gameObject));
    }
}

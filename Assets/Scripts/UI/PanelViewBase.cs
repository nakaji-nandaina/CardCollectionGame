using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelViewBase : MonoBehaviour
{
    [SerializeField] ViewAnimatorHub animatorHub;
    [SerializeField] ViewAnimKey _inAnim = ViewAnimKey.Pop;
    [SerializeField] ViewAnimKey _outAnim = ViewAnimKey.Pop;

    RectTransform rt;
    CanvasGroup cg;
    Tween current;

    protected virtual void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
        if (!cg)
            cg = gameObject.AddComponent<CanvasGroup>();
    }

    protected virtual void OnDestroy() => current?.Kill();

    public void Show(ViewAnimKey? key = null)
    {
        KillTween();
        gameObject.SetActive(true);
        SetInteractable(false);

        ViewAnimKey k = key ?? _inAnim;
        current = animatorHub?.PlayIn(k, rt, cg)
            ?.OnComplete(() => { SetInteractable(true); });
    }

    public void Hide(ViewAnimKey? key = null)
    {
        KillTween();
        SetInteractable(false);

        ViewAnimKey k = key ?? _outAnim;
        current = animatorHub?.PlayOut(k, rt, cg)
            ?.OnComplete(() => { gameObject.SetActive(false); });
    }

    void SetInteractable(bool on)
    {
        cg.interactable = on;
        cg.blocksRaycasts = on;
    }

    void KillTween() {
        current?.Kill(true);
        current = null; 
    }

}

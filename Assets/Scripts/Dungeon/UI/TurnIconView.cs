using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TurnIconView : MonoBehaviour
{
    [SerializeField, Header("ユニットアイコン")] private Image _iconImage;
    [SerializeField, Header("アイコン背景")] private Image _iconBackground;
    [SerializeField, Header("アイコンフレーム")] private Image _iconFrame;
    //#870404
    private readonly Color _enemyColor = new Color(0.529f, 0.016f, 0.016f);
    //#55A4A6
    private readonly Color _playerColor = new Color(0.333f, 0.643f, 0.651f);
    //#F8F074
    private readonly Color _activeFrameColor = new Color(0.973f, 0.941f, 0.455f);

    // 現在割り当てられているユニットの InstanceId
    public string InstanceId { get; private set; }

    // 初期化（null でクリア）
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

    // RectTransform 取得
    public RectTransform GetRectTransform()
    {
        return transform as RectTransform;
    }

    // CanvasGroup 取得（なければ追加）
    public CanvasGroup GetCanvasGroup()
    {
        var cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    // クリア（非表示にして InstanceId をクリア）
    public void Clear()
    {
        InstanceId = null;
        var rt = GetRectTransform();
        var cg = GetCanvasGroup();
        if (rt != null) rt.DOKill(true);
        if (cg != null) cg.DOKill(true);
        gameObject.SetActive(false);
    }

    // 指定位置へ滑らかに移動（通常移動用）
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

    // 割り込み（cut-in）：上から降りてきて target に着地する表現
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

    // 死亡アニメ：現在の見た目を複製して上昇フェードアウトさせる。オリジナルは即時クリアして再利用可能にする。
    public void PlayRemoveAsClone(float duration, float offsetY)
    {
        var originalRt = GetRectTransform();
        var originalCg = GetCanvasGroup();

        // 複製を作る（親は同じ）
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

        // start alpha: 元がほぼ透明なら見えなくなるのを防ぐ（最低限可視にする）
        float startAlpha = 1f;
        if (originalCg != null) startAlpha = originalCg.alpha;
        if (startAlpha < 0.05f) startAlpha = 1f;
        cloneCg.alpha = startAlpha;

        // オリジナルは即時クリアして他で再利用可能にする
        if (originalRt != null) originalRt.DOKill(true);
        if (originalCg != null) originalCg.DOKill(true);
        Clear();

        // 複製を上へスライド＆フェードアウトして破棄
        Sequence seq = DOTween.Sequence();
        seq.Append(cloneCg.DOFade(0f, duration));
        if (cloneRt != null)
        {
            seq.Join(cloneRt.DOAnchorPos(cloneRt.anchoredPosition + new Vector2(0, offsetY), duration).SetEase(Ease.InBack));
        }
        seq.OnComplete(() => Destroy(clone));
    }

    /// <summary>
    /// 見た目だけ複製して targetPos へ移動させる。オリジナルは即時 Clear されスロットは即割り当て可能になる。
    /// cutIn: true の場合は startOffsetY を利用して開始位置を上（+Y）にずらしフェードインして着地。
    /// fadeOutOnMove: true の時は移動中にフェードアウトして破棄する（移動で消える表現）。
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

        // 複製作成
        GameObject clone = Instantiate(gameObject, transform.parent);
        var cloneRt = clone.GetComponent<RectTransform>();
        var cloneCg = clone.GetComponent<CanvasGroup>();
        if (cloneCg == null) cloneCg = clone.AddComponent<CanvasGroup>();

        // 初期状態セット
        cloneRt.anchoredPosition = originalRt.anchoredPosition + (cutIn ? new Vector2(0, startOffsetY) : Vector2.zero);
        cloneRt.localScale = originalRt.localScale;
        cloneRt.localRotation = originalRt.localRotation;

        // 複製は確実に可視で開始する（元がフェード中で alpha が小さい状態をコピーしない）
        cloneCg.alpha = 1f;

        // オリジナルは即時クリアして再利用可能にする
        originalRt.DOKill(true);
        if (originalCg != null) originalCg.DOKill(true);
        Clear();

        // アニメ実行
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
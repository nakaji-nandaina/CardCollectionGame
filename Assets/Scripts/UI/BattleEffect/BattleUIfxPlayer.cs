using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
public enum BattleUIfxType
{
    Slash,
    Explosion
}

public class BattleUIfxPlayer : MonoBehaviour
{
    [SerializeField, Header("VFX配置先(同一Canvas内)")] private RectTransform _overlayRoot;

    [SerializeField, Header("斬撃プレハブ")] private BaseUIfx _slashPrefab;
    [SerializeField, Header("爆発プレハブ")] private BaseUIfx _explosionPrefab;
    public void PlayAt(RectTransform target, BattleUIfxType type, float size = 1.0f)
    {
        if (target == null || _overlayRoot == null) 
            return;

        BaseUIfx prefab = GetPrefab(type);
        if (prefab == null) 
            return;

        GameObject go = Instantiate(prefab.gameObject, _overlayRoot);
        var rt = go.GetComponent<RectTransform>();
        if (rt == null) 
            rt = go.AddComponent<RectTransform>();

        // target座標をoverlayRoot座標系に変換して設置
        Vector2 anchored = DOTweenModuleUI.Utils.SwitchToRectTransform(target, _overlayRoot);
        rt.anchoredPosition = anchored;
        rt.localScale = Vector3.one * size;
        rt.localRotation = Quaternion.identity;

        go.GetComponent<BaseUIfx>().PlayAnimation();
    }

    // rt を直接変更せず、指定したワールド座標に VFX を重ねて再生します。
    public void PlayAt(Vector3 worldPos, BattleUIfxType type, Camera worldToScreenCamera = null, float size = 1.0f)
    {
        if (_overlayRoot == null) return;

        BaseUIfx prefab = GetPrefab(type);
        if (prefab == null) return;

        // ワールド -> スクリーン
        Camera worldCam = worldToScreenCamera ?? Camera.main;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(worldCam, worldPos);

        // overlay の Canvas とカメラ取得（ScreenSpace-Overlay の場合は null）
        Canvas dstCanvas = _overlayRoot.GetComponentInParent<Canvas>();
        Camera dstCam = (dstCanvas != null && dstCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? dstCanvas.worldCamera : null;

        // スクリーン座標 -> overlay のローカル座標
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_overlayRoot, screenPoint, dstCam, out Vector2 localPoint);

        GameObject go = Instantiate(prefab.gameObject, _overlayRoot);
        var rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();

        rt.anchoredPosition = localPoint;
        rt.localScale = Vector3.one * size;
        rt.localRotation = Quaternion.identity;
        rt.SetAsLastSibling();

        go.GetComponent<BaseUIfx>().PlayAnimation();
    }

    private BaseUIfx GetPrefab(BattleUIfxType type)
    {
        switch (type)
        {
            case BattleUIfxType.Slash: return _slashPrefab;
            case BattleUIfxType.Explosion: return _explosionPrefab;
            default: return null;
        }
    }
}

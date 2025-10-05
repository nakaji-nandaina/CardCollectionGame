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
    [SerializeField, Header("VFX�z�u��(����Canvas��)")] private RectTransform _overlayRoot;

    [SerializeField, Header("�a���v���n�u")] private BaseUIfx _slashPrefab;
    [SerializeField, Header("�����v���n�u")] private BaseUIfx _explosionPrefab;
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

        // target���W��overlayRoot���W�n�ɕϊ����Đݒu
        Vector2 anchored = DOTweenModuleUI.Utils.SwitchToRectTransform(target, _overlayRoot);
        rt.anchoredPosition = anchored;
        rt.localScale = Vector3.one * size;
        rt.localRotation = Quaternion.identity;

        go.GetComponent<BaseUIfx>().PlayAnimation();
    }

    // rt �𒼐ڕύX�����A�w�肵�����[���h���W�� VFX ���d�˂čĐ����܂��B
    public void PlayAt(Vector3 worldPos, BattleUIfxType type, Camera worldToScreenCamera = null, float size = 1.0f)
    {
        if (_overlayRoot == null) return;

        BaseUIfx prefab = GetPrefab(type);
        if (prefab == null) return;

        // ���[���h -> �X�N���[��
        Camera worldCam = worldToScreenCamera ?? Camera.main;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(worldCam, worldPos);

        // overlay �� Canvas �ƃJ�����擾�iScreenSpace-Overlay �̏ꍇ�� null�j
        Canvas dstCanvas = _overlayRoot.GetComponentInParent<Canvas>();
        Camera dstCam = (dstCanvas != null && dstCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? dstCanvas.worldCamera : null;

        // �X�N���[�����W -> overlay �̃��[�J�����W
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

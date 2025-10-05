using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// �A�j���[�V����SO���L�[�ŊǗ�����n�u
/// ���ꂼ���View��1���A�^�b�`���Ďg��
/// </summary>
public enum ViewAnimKey {
    None,
    Pop,
    Slide,
    Fade
}
public class ViewAnimatorHub : MonoBehaviour
{
    [System.Serializable]
    public struct Entry
    {
        public ViewAnimKey key;        // ���ʃL�[
        public ViewAnimatorSO animator; // ���ۂ�SO
    }

    [SerializeField] Entry[] entries;  // �C���X�y�N�^�œo�^
    Dictionary<ViewAnimKey, ViewAnimatorSO> map;

    void Awake()
    {
        map = entries?.Where(e => e.animator != null)
                      .ToDictionary(e => e.key, e => e.animator)
              ?? new Dictionary<ViewAnimKey, ViewAnimatorSO>();
    }

    public Tween PlayIn(ViewAnimKey key, RectTransform rt, CanvasGroup cg)
        => map.TryGetValue(key, out var a) ? a.PlayIn(rt, cg) : null;

    public Tween PlayOut(ViewAnimKey key, RectTransform rt, CanvasGroup cg)
        => map.TryGetValue(key, out var a) ? a.PlayOut(rt, cg) : null;
}

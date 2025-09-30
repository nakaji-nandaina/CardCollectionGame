using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreView : MonoBehaviour
{
    [SerializeField, Header("�G���j�b�g�摜")] private List<Image> _enemyImage;
    [SerializeField, Header("�G���j�b�gHP�o�[")] private List<Slider> _enemyHpbar;
    [SerializeField, Header("�G���j�b�gHP�e�L�X�g")] private List<Text> _enemyHpText;
    [SerializeField, Header("�w�i�摜")] private Image _backgroundImage;
    [SerializeField, Header("�K�w�e�L�X�g")] private Text _floorText;

    private int _maxHp;

    public void SetUp(List<EnemyUnitData> enemyUnitData)
    {
        _backgroundImage.sprite = DungeonManager.Instance.CurrentDungeonData.BackgroundImage;
        for (int i = 0; i < enemyUnitData.Count; i++)
        {
            _enemyImage[i].sprite = enemyUnitData[i].FullSprite;
            _maxHp = enemyUnitData[i].MaxHealth;
            _enemyHpbar[i].maxValue = _maxHp;
            _enemyHpbar[i].value = _maxHp;
            _enemyHpText[i].text = $"{_maxHp}/{_maxHp}";
        }
    }
    public void UpdateEnemy(int index, int currentHp)
    {
        _enemyHpbar[index].value = currentHp;
        _enemyHpText[index].text = $"{currentHp}/{_maxHp}";
    }
    public void PlayEnemyDamagedAnimation(int index)
    {
        Transform t = _enemyImage[index].transform;
        RectTransform rt = t as RectTransform;
        rt.DOKill(true); // �����̃A�j�����~
        float offsetX = 10f;
        float duration = 0.05f; // ���E1�����̕Г�����

        Vector2 start = rt.anchoredPosition;
        // �V�[�P���X�ō��E�ɐU��
        Sequence seq = DOTween.Sequence();
        // ����
        seq.Append(rt.DOAnchorPos(start + new Vector2(-offsetX, 0), duration));
        // �E��
        seq.Append(rt.DOAnchorPos(start + new Vector2(offsetX, 0), duration));
        // �߂�
        seq.Append(rt.DOAnchorPos(start, duration));
        // �J��Ԃ��񐔂�ݒ�i�����ł�2�񉝕��j
        seq.SetLoops(2, LoopType.Yoyo);
        // Ease�ŗh����ۂ������o
        seq.SetEase(Ease.InOutQuad);
    }

    public void UpdateFloor(int floorNum,Action OnFinished)
    {
        _floorText.text = $"{floorNum}F";
        OnFinished?.Invoke();
    }
}

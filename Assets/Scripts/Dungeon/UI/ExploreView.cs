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
    //�A�j���[�V�����Đ�����
    private float _entranceAnimDuration = 0.8f;
    private float _deadAnimDuration = 0.3f;
    private float _damagedAnimDuration = 0.05f;
    private float _attackAnimDuration = 0.3f;

    /// <summary>
    /// �G�f�[�^���g����UI���Z�b�g�A�b�v
    /// </summary>
    /// <param name="enemyUnitData"></param>
    public void SetUp(List<EnemyUnitData> enemyUnitData)
    {
        _backgroundImage.sprite = DungeonManager.Instance.CurrentDungeonData.BackgroundImage;
        for (int i = 0; i < enemyUnitData.Count; i++)
        {
            _enemyImage[i].DOKill(true); // �����̃A�j�����~
            _enemyHpbar[i].DOKill(true); // �����̃A�j�����~
            _enemyImage[i].gameObject.SetActive(true);
            _enemyHpbar[i].gameObject.SetActive(true);

            _enemyImage[i].sprite = enemyUnitData[i].FullSprite;
            _maxHp = enemyUnitData[i].MaxHealth;
            _enemyHpbar[i].maxValue = _maxHp;
            _enemyHpbar[i].value = _maxHp;
            _enemyHpText[i].text = $"{_maxHp}/{_maxHp}";
            _enemyImage[i].rectTransform.localScale = Vector2.zero;
        }
    }
    /// <summary>
    /// �G�̓o��A�j���[�V����
    /// </summary>
    public void PlayEnemyEntranceAnimation()
    {
        for (int i = 0; i < _enemyImage.Count; i++)
        {
            Transform t = _enemyImage[i].transform;
            RectTransform rt = t as RectTransform;
            rt.DOKill(true); // �����̃A�j�����~
            Vector2 originalScale = Vector2.one;
            rt.localScale = Vector2.zero; // �����X�P�[����0�ɐݒ�
            rt.DOScale(originalScale, _entranceAnimDuration).SetEase(Ease.OutBack); // �X�P�[���A�b�v�A�j���[�V����
        }
    }

    /// <summary>
    /// �G��HP�o�[�ƃe�L�X�g���X�V
    /// </summary>
    /// <param name="index"></param>
    /// <param name="currentHp"></param>
    public void UpdateEnemy(int index, int currentHp)
    {
        _enemyHpbar[index].value = currentHp;
        _enemyHpText[index].text = $"{currentHp}/{_maxHp}";
    }
    /// <summary>
    /// �G���_���[�W���󂯂��Ƃ��̃A�j���[�V����
    /// </summary>
    /// <param name="index"></param>
    public void PlayEnemyDamagedAnimation(int index)
    {
        Transform t = _enemyImage[index].transform;
        RectTransform rt = t as RectTransform;
        rt.DOKill(true); // �����̃A�j�����~
        float offsetX = 10f;

        Vector2 start = rt.anchoredPosition;
        // �V�[�P���X�ō��E�ɐU��
        Sequence seq = DOTween.Sequence();
        // ����
        seq.Append(rt.DOAnchorPos(start + new Vector2(-offsetX, 0), _damagedAnimDuration));
        // �E��
        seq.Append(rt.DOAnchorPos(start + new Vector2(offsetX, 0), _damagedAnimDuration));
        // �߂�
        seq.Append(rt.DOAnchorPos(start, _damagedAnimDuration));
        // �J��Ԃ��񐔂�ݒ�i�����ł�2�񉝕��j
        seq.SetLoops(2, LoopType.Yoyo);
        // Ease�ŗh����ۂ������o
        seq.SetEase(Ease.InOutQuad);
    }

    public void PlayEnemyAttackAnimation(int index)
    {
        Transform t = _enemyImage[index].transform;
        RectTransform rt = t as RectTransform;
        rt.DOKill(true); // �����̃A�j�����~
        float offsetY = 20f;

        Vector2 start = rt.anchoredPosition;
        // ���Ɉړ����Ė߂�A�j���[�V����
        Sequence seq = DOTween.Sequence();
        // ����
        seq.Append(rt.DOAnchorPos(start + new Vector2(0, -offsetY), _attackAnimDuration));
        // �߂�
        seq.Append(rt.DOAnchorPos(start, _attackAnimDuration));
    }

    public void PlayEnemyDeadAnimation(int index)
    {
        Transform t = _enemyImage[index].transform;
        RectTransform rt = t as RectTransform;
        rt.DOKill(true); // �����̃A�j�����~
        Vector2 targetScale = Vector2.zero;
        rt.localScale = Vector2.one; // �����X�P�[����0�ɐݒ�
        rt.DOScale(targetScale, _deadAnimDuration)
          .SetDelay(0.2f)
          .SetEase(Ease.Linear)
          .OnComplete(() =>
          {
              _enemyImage[index].gameObject.SetActive(false);
              _enemyHpbar[index].gameObject.SetActive(false);
          }); // �X�P�[���A�b�v�A�j���[�V����
    }

    /// <summary>
    /// �K�w�e�L�X�g�X�V
    /// </summary>
    /// <param name="floorNum"></param>
    /// <param name="OnFinished"></param>
    public void UpdateFloor(int floorNum,Action OnFinished)
    {
        _floorText.text = $"{floorNum}F";
        OnFinished?.Invoke();
    }

    /// <summary>
    /// �G���j�b�g��RectTransform���擾
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool TryGetEnemyRectTransform(int index, out RectTransform rt)
    {
        rt = null;
        if (index < 0 || index >= _enemyImage.Count) return false;
        rt = _enemyImage[index].transform as RectTransform;
        return true;
    }
}

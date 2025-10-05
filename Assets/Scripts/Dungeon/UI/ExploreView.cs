using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreView : MonoBehaviour
{
    [SerializeField, Header("敵ユニット画像")] private List<Image> _enemyImage;
    [SerializeField, Header("敵ユニットHPバー")] private List<Slider> _enemyHpbar;
    [SerializeField, Header("敵ユニットHPテキスト")] private List<Text> _enemyHpText;
    [SerializeField, Header("背景画像")] private Image _backgroundImage;
    [SerializeField, Header("階層テキスト")] private Text _floorText;

    private int _maxHp;
    //アニメーション再生時間
    private float _entranceAnimDuration = 0.8f;
    private float _deadAnimDuration = 0.3f;
    private float _damagedAnimDuration = 0.05f;
    private float _attackAnimDuration = 0.3f;

    /// <summary>
    /// 敵データを使ってUIをセットアップ
    /// </summary>
    /// <param name="enemyUnitData"></param>
    public void SetUp(List<EnemyUnitData> enemyUnitData)
    {
        _backgroundImage.sprite = DungeonManager.Instance.CurrentDungeonData.BackgroundImage;
        for (int i = 0; i < enemyUnitData.Count; i++)
        {
            _enemyImage[i].DOKill(true); // 既存のアニメを停止
            _enemyHpbar[i].DOKill(true); // 既存のアニメを停止
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
    /// 敵の登場アニメーション
    /// </summary>
    public void PlayEnemyEntranceAnimation()
    {
        for (int i = 0; i < _enemyImage.Count; i++)
        {
            Transform t = _enemyImage[i].transform;
            RectTransform rt = t as RectTransform;
            rt.DOKill(true); // 既存のアニメを停止
            Vector2 originalScale = Vector2.one;
            rt.localScale = Vector2.zero; // 初期スケールを0に設定
            rt.DOScale(originalScale, _entranceAnimDuration).SetEase(Ease.OutBack); // スケールアップアニメーション
        }
    }

    /// <summary>
    /// 敵のHPバーとテキストを更新
    /// </summary>
    /// <param name="index"></param>
    /// <param name="currentHp"></param>
    public void UpdateEnemy(int index, int currentHp)
    {
        _enemyHpbar[index].value = currentHp;
        _enemyHpText[index].text = $"{currentHp}/{_maxHp}";
    }
    /// <summary>
    /// 敵がダメージを受けたときのアニメーション
    /// </summary>
    /// <param name="index"></param>
    public void PlayEnemyDamagedAnimation(int index)
    {
        Transform t = _enemyImage[index].transform;
        RectTransform rt = t as RectTransform;
        rt.DOKill(true); // 既存のアニメを停止
        float offsetX = 10f;

        Vector2 start = rt.anchoredPosition;
        // シーケンスで左右に振動
        Sequence seq = DOTween.Sequence();
        // 左へ
        seq.Append(rt.DOAnchorPos(start + new Vector2(-offsetX, 0), _damagedAnimDuration));
        // 右へ
        seq.Append(rt.DOAnchorPos(start + new Vector2(offsetX, 0), _damagedAnimDuration));
        // 戻る
        seq.Append(rt.DOAnchorPos(start, _damagedAnimDuration));
        // 繰り返し回数を設定（ここでは2回往復）
        seq.SetLoops(2, LoopType.Yoyo);
        // Easeで揺れっぽさを演出
        seq.SetEase(Ease.InOutQuad);
    }

    public void PlayEnemyAttackAnimation(int index)
    {
        Transform t = _enemyImage[index].transform;
        RectTransform rt = t as RectTransform;
        rt.DOKill(true); // 既存のアニメを停止
        float offsetY = 20f;

        Vector2 start = rt.anchoredPosition;
        // 下に移動して戻るアニメーション
        Sequence seq = DOTween.Sequence();
        // 下へ
        seq.Append(rt.DOAnchorPos(start + new Vector2(0, -offsetY), _attackAnimDuration));
        // 戻る
        seq.Append(rt.DOAnchorPos(start, _attackAnimDuration));
    }

    public void PlayEnemyDeadAnimation(int index)
    {
        Transform t = _enemyImage[index].transform;
        RectTransform rt = t as RectTransform;
        rt.DOKill(true); // 既存のアニメを停止
        Vector2 targetScale = Vector2.zero;
        rt.localScale = Vector2.one; // 初期スケールを0に設定
        rt.DOScale(targetScale, _deadAnimDuration)
          .SetDelay(0.2f)
          .SetEase(Ease.Linear)
          .OnComplete(() =>
          {
              _enemyImage[index].gameObject.SetActive(false);
              _enemyHpbar[index].gameObject.SetActive(false);
          }); // スケールアップアニメーション
    }

    /// <summary>
    /// 階層テキスト更新
    /// </summary>
    /// <param name="floorNum"></param>
    /// <param name="OnFinished"></param>
    public void UpdateFloor(int floorNum,Action OnFinished)
    {
        _floorText.text = $"{floorNum}F";
        OnFinished?.Invoke();
    }

    /// <summary>
    /// 敵ユニットのRectTransformを取得
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

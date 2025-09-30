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
        rt.DOKill(true); // 既存のアニメを停止
        float offsetX = 10f;
        float duration = 0.05f; // 左右1往復の片道時間

        Vector2 start = rt.anchoredPosition;
        // シーケンスで左右に振動
        Sequence seq = DOTween.Sequence();
        // 左へ
        seq.Append(rt.DOAnchorPos(start + new Vector2(-offsetX, 0), duration));
        // 右へ
        seq.Append(rt.DOAnchorPos(start + new Vector2(offsetX, 0), duration));
        // 戻る
        seq.Append(rt.DOAnchorPos(start, duration));
        // 繰り返し回数を設定（ここでは2回往復）
        seq.SetLoops(2, LoopType.Yoyo);
        // Easeで揺れっぽさを演出
        seq.SetEase(Ease.InOutQuad);
    }

    public void UpdateFloor(int floorNum,Action OnFinished)
    {
        _floorText.text = $"{floorNum}F";
        OnFinished?.Invoke();
    }
}

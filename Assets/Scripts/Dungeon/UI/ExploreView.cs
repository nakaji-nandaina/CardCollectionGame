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
    public void PlayEnemyDamagedAnimation()
    {
        //DOTween導入後に実装
    }
}

using UnityEngine;
using UnityEngine.UI;

public class BattleCardView : MonoBehaviour
{
    [SerializeField, Header("カード画像")] private Image _cardImage;
    [SerializeField, Header("カード名")] private Text _cardName;
    [SerializeField, Header("背景")] private Image _background;
    [SerializeField, Header("カードフレーム")] private Image _cardFrame;
    [SerializeField, Header("HPバー")] private Slider _hpBar;
    [SerializeField, Header("MPバー")] private Slider _mpBar;
    [SerializeField, Header("HPテキスト")] private Text _hpText;
    [SerializeField, Header("MPテキスト")] private Text _mpText;

    private string _instanceId;
    public string InstanceId => _instanceId;

    public void SetUp(UnitSlot unit)
    {
        _cardImage.sprite = unit.UnitData.FaceSprite;
        _cardName.text = unit.UnitData.UnitName;
        _hpBar.maxValue = unit.MaxHp;
        _hpBar.value = unit.CurrentHp;
        _mpBar.maxValue = unit.MaxMp;
        _mpBar.value = unit.CurrentMp;
        gameObject.SetActive(true);
    }
    public void UpdateCurrent(int currentHp, int currentMp)
    {
        if (_hpBar != null) _hpBar.value = Mathf.Clamp(currentHp, 0, (int)_hpBar.maxValue);
        if (_mpBar != null) _mpBar.value = Mathf.Clamp(currentMp, 0, (int)_mpBar.maxValue);
        if (_hpText != null) _hpText.text = $"{_hpBar.value}/{_hpBar.maxValue}";
        if (_mpText != null) _mpText.text = $"{_mpBar.value}/{_mpBar.maxValue}";
    }


}

using UnityEngine;
using UnityEngine.UI;

public class BattleCardView : MonoBehaviour
{
    [SerializeField, Header("�J�[�h�摜")] private Image _cardImage;
    [SerializeField, Header("�J�[�h��")] private Text _cardName;
    [SerializeField, Header("�w�i")] private Image _background;
    [SerializeField, Header("�J�[�h�t���[��")] private Image _cardFrame;
    [SerializeField, Header("HP�o�[")] private Slider _hpBar;
    [SerializeField, Header("MP�o�[")] private Slider _mpBar;
    [SerializeField, Header("HP�e�L�X�g")] private Text _hpText;
    [SerializeField, Header("MP�e�L�X�g")] private Text _mpText;

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
        _hpText.text = $"{_hpBar.value}/{_hpBar.maxValue}";
        _mpText.text = $"{_mpBar.value}/{_mpBar.maxValue}";
        gameObject.SetActive(true);
    }
    public void SetUp(BattleUnit unit)
    {
        _instanceId = unit.InstanceId;
        _cardImage.sprite = unit.Data.FaceSprite;
        _cardName.text = unit.Data.UnitName;
        _hpBar.maxValue = unit.MaxHp;
        _hpBar.value = unit.CurrentHp;
        _mpBar.maxValue = unit.MaxMp;
        _mpBar.value = unit.CurrentMp;
        _hpText.text = $"{_hpBar.value}/{_hpBar.maxValue}";
        _mpText.text = $"{_mpBar.value}/{_mpBar.maxValue}";
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

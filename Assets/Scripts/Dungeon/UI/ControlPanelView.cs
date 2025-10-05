using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanelView : MonoBehaviour
{
    [SerializeField, Header("�Q�[�����x�X���C�_�[")] Slider _speedSlider;
    [SerializeField, Header("�Q�[�����x�e�L�X�g")] Text _speedText;
    [SerializeField, Header("�ꎞ��~�{�^��")] Button _pauseButton;
    [SerializeField, Header("�ꎞ��~�{�^���e�L�X�g")] Text _pauseButtonText;
    [SerializeField, Header("���^�C�A�{�^��")] Button _retireButton;

    string pauseText = "�ꎞ��~";
    string playText = "�ĊJ";

    /// <summary>
    /// �_���W������ʂ̃R���g���[���p�l�����Z�b�g�A�b�v����
    /// </summary>
    /// <param name="OnSliderChanged"></param>
    /// <param name="OnPauseButtonPressed"></param>
    /// <param name="OnRetireButtonPressed"></param>
    /// <param name="initSpeed"></param>
    public void SetUp(Action<float> OnSliderChanged, Action OnPauseButtonPressed, Action OnRetireButtonPressed, float initSpeed)
    {
        _speedSlider.maxValue = Const.Time.MaxSpeed;
        _speedSlider.minValue = Const.Time.MinSpeed;
        _speedSlider.onValueChanged.RemoveAllListeners();
        _speedSlider.onValueChanged.AddListener((value) =>
        {
            OnSliderChanged(value);
            _speedText.text = $"{value:0.0}x";
        });
        _speedSlider.value = initSpeed;
        _speedText.text = $"{initSpeed:0.0}x";

        _pauseButton.onClick.RemoveAllListeners();
        _pauseButton.onClick.AddListener(() => OnPauseButtonPressed());
        
        _retireButton.onClick.RemoveAllListeners();
        _retireButton.onClick.AddListener(() => OnRetireButtonPressed());
    }

    public void SetPause(bool isPouse)
    {
        _pauseButtonText.text = isPouse ? playText : pauseText;
        _speedSlider.interactable = !isPouse;
    }
}

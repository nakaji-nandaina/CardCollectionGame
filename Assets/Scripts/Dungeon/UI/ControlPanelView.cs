using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanelView : MonoBehaviour
{
    [SerializeField, Header("ゲーム速度スライダー")] Slider _speedSlider;
    [SerializeField, Header("ゲーム速度テキスト")] Text _speedText;
    [SerializeField, Header("一時停止ボタン")] Button _pauseButton;
    [SerializeField, Header("一時停止ボタンテキスト")] Text _pauseButtonText;
    [SerializeField, Header("リタイアボタン")] Button _retireButton;

    string pauseText = "一時停止";
    string playText = "再開";

    /// <summary>
    /// ダンジョン画面のコントロールパネルをセットアップする
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

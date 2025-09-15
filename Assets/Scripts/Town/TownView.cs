using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownView : MonoBehaviour
{
    [SerializeField, Header("�X�^�[�g�{�^��")] private Button _startButton;
    public void SetUp(Action OnStartButtonPressed)
    {
        _startButton.onClick.RemoveAllListeners();
        _startButton.onClick.AddListener(() => OnStartButtonPressed());
    }
}

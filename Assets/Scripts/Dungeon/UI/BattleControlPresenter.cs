using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleControlPresenter : MonoBehaviour
{
    private ControlPanelView _controlPanelView;
    float _lastSpeed;
    bool _isPause = false;

    private void OnEnable()
    {
        _controlPanelView = FindAnyObjectByType<ControlPanelView>();
        _controlPanelView.SetUp(
            OnSliderChanged: ChangeBattleSpeed,
            OnPauseButtonPressed: SetPause,
            OnRetireButtonPressed: Retire,
            initSpeed: Time.timeScale
            );
    }

    /// <summary>
    /// �_���W�����̐i�s���x��ύX����
    /// </summary>
    /// <param name="speed"></param>
    private void ChangeBattleSpeed(float speed)
    {
        _lastSpeed = speed;
        DungeonManager.Instance.SetPlaySpeed(speed);
    }

    /// <summary>
    /// �_���W�����̐i�s���x���ꎞ��~�܂��͌��ɖ߂�
    /// </summary>
    private void SetPause()
    {
        if(!_isPause)
        {
            _lastSpeed = Time.timeScale;
            DungeonManager.Instance.StopPlaySpeed();
            _isPause = true;
            _controlPanelView.SetPause(_isPause);
        }
        else
        {
            DungeonManager.Instance.SetPlaySpeed(_lastSpeed);
            _isPause = false;
            _controlPanelView.SetPause(_isPause);
        }
    }

    private void Retire()
    {
        DungeonManager.Instance.HandleRetire();
    }
}

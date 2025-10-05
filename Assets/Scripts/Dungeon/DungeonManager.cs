using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DungeonState
{
    None,
    DungeonStart,
    FloorStart,
    Battle,
    FloorCleared,
    DungeonCleared,
    FloorEnd,
    Failed,
}

/// <summary>
/// �_���W�������̏�ԊǗ����s���}�l�[�W���[�N���X
/// </summary>
public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }
    private DungeonState _currentState = DungeonState.None;
    private int _currentFloor = 1;  // ���݂̃t���A
    private int _gotGold = 0;       // �l�������S�[���h
    private float _playSpeed = 1.0f;// �v���C���x
    private float _defaultFixedDeltaTime; // �����X�e�b�v�̏����l�ێ�

    private List<PlayerUnitData> _gotUnitDataList = new List<PlayerUnitData>(); // �l���������j�b�g�f�[�^���X�g
    private DungeonData _currentDungeonData = null; // ���݂̃_���W�����f�[�^
    private Coroutine _dungeonLoop;

    public DungeonState CurrentState => _currentState;
    public int CurrentFloor => _currentFloor;
    public int GotGold => _gotGold;
    public float PlaySpeed => _playSpeed;
    public List<PlayerUnitData> GotUnitDataList => _gotUnitDataList;
    public DungeonData CurrentDungeonData => _currentDungeonData;
    public Action<DungeonState> OnDungeonStateChanged;
    
    private void Awake()
    {
        Instance = this;
        _defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void ApplyPlaySpeed()
    {
        Time.timeScale = _playSpeed;
        DOTween.timeScale = _playSpeed;
        Time.fixedDeltaTime = _defaultFixedDeltaTime * _playSpeed;
    }

    private void ResetPlaySpeed()
    {
        Time.timeScale = Const.Time.DefaultSpeed;
        DOTween.timeScale = Const.Time.DefaultSpeed;
        Time.fixedDeltaTime = _defaultFixedDeltaTime;
    }

    public void SetPlaySpeed(float speed)
    {
        speed = Mathf.Clamp(speed, Const.Time.MinSpeed, Const.Time.MaxSpeed);
        if (Mathf.Approximately(_playSpeed, speed))
            return;

        _playSpeed = speed;
        ApplyPlaySpeed();
    }

    public void StopPlaySpeed()
    {
        _playSpeed = 0f;
        ApplyPlaySpeed();
    }

    private void ChangeState(DungeonState newState)
    {
        if (_currentState != newState)
        {
            _currentState = newState;
            OnDungeonStateChanged?.Invoke(newState);
            //Debug.Log($"Dungeon State changed to: {_currentState}");
        }
    }


    public void StartDungeon(DungeonData dungeonData)
    {
        if(dungeonData == null)
        {
            Debug.LogError("DungeonData is null. Cannot start dungeon.");
            return;
        }
        if(_dungeonLoop != null)
        {
            Debug.LogWarning("Dungeon is already in progress.");
            return;
        }
        _currentDungeonData = dungeonData;
        _currentFloor = 1;
        _gotGold = 0;
        _dungeonLoop = StartCoroutine(DungeonLoop());

    }

    /// <summary>
    /// �t���A�J�n�A�j���[�V�����̊����ʒm�󂯎��
    /// </summary>
    public void NotifyFloorStartAnimationFinished()
    {
        ChangeState(DungeonState.Battle);
    }
    public void NotifyFloorClearedAnimationFinished()
    {
        //�s�k���� FloorCleared �ɂȂ�Ȃ��̂Ŗ���
        if (_currentState != DungeonState.FloorCleared)
        {
            return;
        }
        ChangeState(DungeonState.FloorEnd);
    }

    public void HandleBattleFinish(BattleResult result)
    {
        switch(result)
        {
            case BattleResult.Victory:
                ChangeState(DungeonState.FloorCleared);
                break;
            case BattleResult.Defeat:
                ChangeState(DungeonState.Failed);
                break;
        }
    }

    public void HandleRetire()
    {
        if(_currentState == DungeonState.Battle || _currentState == DungeonState.FloorStart)
        {
            ChangeState(DungeonState.Failed);
        }
    }

    private IEnumerator DungeonLoop()
    {
        ChangeState(DungeonState.DungeonStart);
        yield return null;

        int maxFloor = _currentDungeonData?.MaxFloor ?? 1;
        for (int floor = _currentFloor; floor <= maxFloor; floor++)
        {
            _currentFloor = floor;

            // �t���A�J�n
            ChangeState(DungeonState.FloorStart);
            yield return null;
            // �o�g���̊J�n��҂�
            yield return new WaitUntil(() => _currentState == DungeonState.Battle);
            // �o�g���̌��ʁiHandleBattleFinish�j�� DungeonState �� InProgress �� Failed �ɑJ�ڂ���z��
            // �����ő҂FBattle ���I���܂ŁiDungeonState �� Battle �łȂ��Ȃ�܂Łj�ҋ@
            yield return new WaitUntil(() => _currentState != DungeonState.Battle);

            if (_currentState == DungeonState.Failed)
            {
                ResetPlaySpeed();
                _dungeonLoop = null;
                yield break;
            }

            yield return new WaitUntil(()=> _currentState == DungeonState.FloorEnd);

            // ���t���A�ֈړ�����O�̒Z����
            yield return new WaitForSeconds(0.5f);
        }

        // �S�t���A�N���A
        ChangeState(DungeonState.DungeonCleared);
        ResetPlaySpeed();
        _dungeonLoop = null;
    }
    private void OnDisable()
    {
        ResetPlaySpeed();
    }
}

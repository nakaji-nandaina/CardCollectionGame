using System;
using System.Collections.Generic;
using UnityEngine;

public enum DungeonState
{
    None,
    DungeonStart,
    FloorStart,
    Battle,
    InProgress,
    FloorCleared,
    DungeonCleared,
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
    private List<PlayerUnitData> _gotUnitDataList = new List<PlayerUnitData>(); // �l���������j�b�g�f�[�^���X�g


    public DungeonState CurrentState => _currentState;
    public int CurrentFloor => _currentFloor;
    public int GotGold => _gotGold;
    public float PlaySpeed => _playSpeed;
    public List<PlayerUnitData> GotUnitDataList => _gotUnitDataList;

    public Action<DungeonState> OnDungeonStateChanged;
    public Action<float> OnPlaySpeedChanged;
    private void OnDisable()
    {
        Instance = null;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeState(DungeonState newState)
    {
        if (_currentState != newState)
        {
            _currentState = newState;
            OnDungeonStateChanged?.Invoke(newState);
            Debug.Log($"Dungeon State changed to: {_currentState}");
        }
    }


    public void HandleDungeonStart()
    {
        ChangeState(DungeonState.DungeonStart);
    }

    public void HandleBattleStart()
    {

    }

    public void HandleBattleWin()
    {
        
    }   

    public void HandleBattleLose()
    {

    }

}

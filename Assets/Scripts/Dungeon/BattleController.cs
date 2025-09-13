using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Idle,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}
public class BattleController : MonoBehaviour
{
    private float TurnDuration = 1.0f;
    private float TurnTimer = 0.0f;

    private BattleState _currentState = BattleState.Idle;

    public event Action<BattleState> OnBattleStateChanged;

    public BattleState CurrentState => _currentState;

    private void ChangeBattleState(BattleState newState)
    {
        if (_currentState != newState)
        {
            _currentState = newState;
            OnBattleStateChanged?.Invoke(newState);
            Debug.Log($"Battle State changed to: {_currentState}");
        }
    }

    void Update()
    {
        
    }
}


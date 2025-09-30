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
/// ダンジョン内の状態管理を行うマネージャークラス
/// </summary>
public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }
    private DungeonState _currentState = DungeonState.None;
    private int _currentFloor = 1;  // 現在のフロア
    private int _gotGold = 0;       // 獲得したゴールド
    private float _playSpeed = 1.0f;// プレイ速度
    private List<PlayerUnitData> _gotUnitDataList = new List<PlayerUnitData>(); // 獲得したユニットデータリスト
    private DungeonData _currentDungeonData = null; // 現在のダンジョンデータ
    private Coroutine _dungeonLoop;

    public DungeonState CurrentState => _currentState;
    public int CurrentFloor => _currentFloor;
    public int GotGold => _gotGold;
    public float PlaySpeed => _playSpeed;
    public List<PlayerUnitData> GotUnitDataList => _gotUnitDataList;
    public DungeonData CurrentDungeonData => _currentDungeonData;

    public Action<DungeonState> OnDungeonStateChanged;
    public Action<float> OnPlaySpeedChanged;
    private void Awake()
    {
        Instance = this;
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
    /// フロア開始アニメーションの完了通知受け取り
    /// </summary>
    public void NotifyFloorStartAnimationFinished()
    {
        ChangeState(DungeonState.Battle);
    }
    public void NotifyFloorClearedAnimationFinished()
    {
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
    private IEnumerator DungeonLoop()
    {
        ChangeState(DungeonState.DungeonStart);
        yield return null;

        int maxFloor = _currentDungeonData?.MaxFloor ?? 1;
        for (int floor = _currentFloor; floor <= maxFloor; floor++)
        {
            _currentFloor = floor;

            // フロア開始
            ChangeState(DungeonState.FloorStart);
            yield return null;
            // バトルの開始を待つ
            yield return new WaitUntil(() => _currentState == DungeonState.Battle);
            // バトルの結果（HandleBattleFinish）で DungeonState が InProgress か Failed に遷移する想定
            // ここで待つ：Battle が終わるまで（DungeonState が Battle でなくなるまで）待機
            yield return new WaitUntil(() => _currentState != DungeonState.Battle);

            if (_currentState == DungeonState.Failed)
            {
                _dungeonLoop = null;
                yield break;
            }

            yield return new WaitUntil(()=> _currentState == DungeonState.FloorEnd);

            // 次フロアへ移動する前の短い間
            yield return new WaitForSeconds(0.5f);
        }

        // 全フロアクリア
        ChangeState(DungeonState.DungeonCleared);
        _dungeonLoop = null;
    }

}

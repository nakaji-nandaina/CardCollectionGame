using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TownState
{
    None,
    Idle,
    Shop,
    Unit,
    Start,
    Dictionary,
    Exit
}

public class TownManager : MonoBehaviour
{
    public static TownManager Instance { get; private set; }
    private TownState _currentState = TownState.None;

    public static Action<TownState> OnTownChangeState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeState(TownState state)
    {
        if(state == _currentState)
        {
            return;
        }

        switch(state)
        {
            case TownState.Idle:
                Debug.Log("待機状態へ");
                break;
            case TownState.Shop:
                Debug.Log("ショップ状態へ");
                break;
            case TownState.Unit:
                Debug.Log("ユニット編成状態へ");
                break;
            case TownState.Start:
                Debug.Log("ゲーム開始状態へ");
                break;
            case TownState.Dictionary:
                Debug.Log("図鑑状態へ");
                break;
            case TownState.Exit:
                Debug.Log("ゲーム終了状態へ");
                break;
            default:
                Debug.LogWarning("不明な状態への遷移が要求されました");
                return;
        }
        OnTownChangeState.Invoke(state);
    }

    public void HandleStartDungeon()
    {
        ChangeState(TownState.Start);
    }

}

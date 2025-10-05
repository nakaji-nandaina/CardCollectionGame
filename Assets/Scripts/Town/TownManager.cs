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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(TownState.Idle);
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
                Debug.Log("�ҋ@��Ԃ�");
                break;
            case TownState.Shop:
                Debug.Log("�V���b�v��Ԃ�");
                break;
            case TownState.Unit:
                Debug.Log("���j�b�g�Ґ���Ԃ�");
                break;
            case TownState.Start:
                Debug.Log("�Q�[���J�n��Ԃ�");
                break;
            case TownState.Dictionary:
                Debug.Log("�}�ӏ�Ԃ�");
                break;
            case TownState.Exit:
                Debug.Log("�Q�[���I����Ԃ�");
                break;
            default:
                Debug.LogWarning("�s���ȏ�Ԃւ̑J�ڂ��v������܂���");
                return;
        }
        OnTownChangeState.Invoke(state);
    }

    public void HandleStartDungeon()
    {
        ChangeState(TownState.Start);
    }

}

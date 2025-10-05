using System.Collections.Generic;
using UnityEngine;

public class BattleLogPresenter : MonoBehaviour
{
    private LogView _logView;
    private BattleController _battleController;

    private void OnEnable()
    {
        // BattleController���V�[��������擾
        _battleController = FindAnyObjectByType<BattleController>();
        // LogView���V�[��������擾���ăZ�b�g�A�b�v
        _logView = FindAnyObjectByType<LogView>();

        _logView.SetUp();
        DungeonManager.Instance.OnDungeonStateChanged += OnDungeonStateChanged;
        _battleController.OnBattlePrepared += OnBattlePrepared;
        _battleController.OnBattleFinished += OnBattleFinished;
        _battleController.OnAttackPerformed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        DungeonManager.Instance.OnDungeonStateChanged -= OnDungeonStateChanged;

        _battleController.OnBattlePrepared -= OnBattlePrepared;
        _battleController.OnBattleFinished -= OnBattleFinished;
        _battleController.OnAttackPerformed -= OnAttackPerformed;
    }

    private void OnDungeonStateChanged(DungeonState state)
    {
        if (_logView == null) return;

        if (state == DungeonState.FloorStart)
        {
            _logView.AddLog("");
            _logView.AddLog($"--- {DungeonManager.Instance.CurrentFloor}F���B ---");
        }
    }

    private void OnBattlePrepared(List<BattleUnit> players, List<BattleUnit> enemies, List<ActionNode> nodes)
    {
        if (_logView == null) 
            return;
        _logView.AddLog("");
        _logView.AddLog("�퓬�J�n�I");
    }

    private void OnBattleFinished(BattleResult result, int exp, List<BattleUnit> players)
    {
        if (_logView == null) 
            return;
        if (result == BattleResult.Victory)
        {
            _logView.AddLog("�퓬�����I");
            _logView.AddLog($"{exp}exp �l��");
        }
        else
        {
            _logView.AddLog("�퓬�s�k...");
        }
    }

    private void OnAttackPerformed(BattleUnit attacker, BattleUnit target, int damage)
    {
        if (_logView == null) 
            return;
        _logView.AddLog($"{attacker.Data.UnitName} �̍U���I {target.Data.UnitName} �� {damage} �̃_���[�W�I");
    }
}
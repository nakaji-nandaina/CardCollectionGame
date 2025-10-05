using System.Collections.Generic;
using UnityEngine;

public class BattleLogPresenter : MonoBehaviour
{
    private LogView _logView;
    private BattleController _battleController;

    private void OnEnable()
    {
        // BattleControllerをシーン内から取得
        _battleController = FindAnyObjectByType<BattleController>();
        // LogViewをシーン内から取得してセットアップ
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
            _logView.AddLog($"--- {DungeonManager.Instance.CurrentFloor}F到達 ---");
        }
    }

    private void OnBattlePrepared(List<BattleUnit> players, List<BattleUnit> enemies, List<ActionNode> nodes)
    {
        if (_logView == null) 
            return;
        _logView.AddLog("");
        _logView.AddLog("戦闘開始！");
    }

    private void OnBattleFinished(BattleResult result, int exp, List<BattleUnit> players)
    {
        if (_logView == null) 
            return;
        if (result == BattleResult.Victory)
        {
            _logView.AddLog("戦闘勝利！");
            _logView.AddLog($"{exp}exp 獲得");
        }
        else
        {
            _logView.AddLog("戦闘敗北...");
        }
    }

    private void OnAttackPerformed(BattleUnit attacker, BattleUnit target, int damage)
    {
        if (_logView == null) 
            return;
        _logView.AddLog($"{attacker.Data.UnitName} の攻撃！ {target.Data.UnitName} に {damage} のダメージ！");
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BattleState
{
    Idle,
    Preparing,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}

public enum BattleResult
{
    None,
    Victory,
    Defeat
}

public class BattleController : MonoBehaviour
{
    private float TurnDuration = 1.0f;
    private float ActionDuration = 0.3f;

    private BattleState _currentState = BattleState.Idle;

    public event Action<BattleState> OnBattleStateChanged;
    public event Action<List<BattleUnit>,List<BattleUnit>> OnBattlePrepared;
    public event Action<BattleUnit, BattleUnit, int> OnAttackPerformed;

    public BattleState CurrentState => _currentState;

    private Coroutine _battleLoop;

    private List<BattleUnit> _players = new List<BattleUnit>();
    private List<BattleUnit> _enemies = new List<BattleUnit>();

    public class BattleUnit
    {
        public string InstanceId;
        public BaseUnitData Data;
        public int MaxHp;
        public int CurrentHp;
        public int MaxMp;
        public int CurrentMp;
        public int AttackPoint;
        public int PhysDefensePoint;
        public int MagicDefensePoint;
        public UnitType Type;
        public bool IsAlive => CurrentHp > 0;

    }

    private void ChangeBattleState(BattleState newState)
    {
        if (_currentState == newState)
        {
            return;
        }
        _currentState = newState;
        switch (newState)
        {
            case BattleState.Preparing:
                PrepareBattle();
                break;
            case BattleState.Victory:
                EndBattle(BattleResult.Victory);
                break;
            case BattleState.Defeat:
                EndBattle(BattleResult.Defeat);
                break;

        }
        OnBattleStateChanged?.Invoke(newState);
        Debug.Log($"Battle State changed to: {_currentState}");        
    }

    private void OnDungeonStateChanged(DungeonState state)
    {
        if(state == DungeonState.Battle)
        {
            ChangeBattleState(BattleState.Preparing);
            
        }
        else
        {
            ChangeBattleState(BattleState.Idle);
        }
    }

    private void Start()
    {
        DungeonManager.Instance.OnDungeonStateChanged += OnDungeonStateChanged;
    }
    private void OnDestroy()
    {
        DungeonManager.Instance.OnDungeonStateChanged -= OnDungeonStateChanged;
    }

 
    private void PrepareBattle()
    {
        _players.Clear();
        _enemies.Clear();
        // プレイヤーユニットの準備
        List<UnitSlot> playerSlot = InventoryManager.Instance.FormedUnitList;
        foreach (var slot in playerSlot)
        {
            BattleUnit player = new BattleUnit
            {
                InstanceId = slot.InstanceId,
                Data = slot.UnitData,
                MaxHp = slot.MaxHp,
                CurrentHp = slot.CurrentHp,
                MaxMp = slot.MaxMp,
                CurrentMp = slot.CurrentMp,
                AttackPoint = slot.AttackPoint,
                PhysDefensePoint = slot.PhysDefensePoint,
                MagicDefensePoint = slot.MagicDefensePoint,
                Type = UnitType.Player
            };
            _players.Add(player);
        }
        // 敵ユニットの準備
        EnemyUnitData enemyUnitData = DungeonManager.Instance.CurrentDungeonData.GetRandomEnemy;
        BattleUnit battleUnit = new BattleUnit
        {
            InstanceId = Guid.NewGuid().ToString(),
            Data = enemyUnitData,
            MaxHp = enemyUnitData.MaxHealth,
            CurrentHp = enemyUnitData.MaxHealth,
            AttackPoint = enemyUnitData.AttackPoint,
            PhysDefensePoint = enemyUnitData.PhysDefensePoint,
            MagicDefensePoint = enemyUnitData.MagicDefensePoint,
            Type = UnitType.Enemy
        };
        _enemies.Add(battleUnit);
        OnBattlePrepared?.Invoke(_players, _enemies);
        _battleLoop = StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        ChangeBattleState(BattleState.PlayerTurn);
        while (true)
        {
            ChangeBattleState(BattleState.PlayerTurn);
            yield return PerformTeamAction(_players, _enemies);
            if (CheckVictory(out var outcome)) 
            {
                ChangeBattleState(outcome == BattleResult.Victory ? BattleState.Victory : BattleState.Defeat);
                yield break; 
            }
            yield return new WaitForSeconds(TurnDuration);
            ChangeBattleState(BattleState.EnemyTurn);
            yield return PerformTeamAction(_enemies, _players);
            if (CheckVictory(out outcome)) 
            {
                ChangeBattleState(outcome == BattleResult.Victory ? BattleState.Victory : BattleState.Defeat);
                yield break;
            }
        }
    }

    private IEnumerator PerformTeamAction(List<BattleUnit> attckers, List<BattleUnit> targets)
    {
        List<BattleUnit> aliveAttackers = attckers.FindAll(u => u.IsAlive);
        foreach (var attacker in aliveAttackers)
        {
            if (targets.TrueForAll(t => !t.IsAlive))
            {
                break; // 全てのターゲットが倒されている場合、攻撃を終了
            }
            List<BattleUnit> aliveTargetList = targets.FindAll(t => t.IsAlive);
            BattleUnit target = aliveTargetList[UnityEngine.Random.Range(0, aliveTargetList.Count)];
            int damage = Mathf.Max(0, attacker.AttackPoint - target.PhysDefensePoint);
            target.CurrentHp = Mathf.Max(0, target.CurrentHp - damage);
            OnAttackPerformed?.Invoke(attacker, target, damage);
            yield return new WaitForSeconds(ActionDuration);
        }
    }

    // 勝敗判定
    private bool CheckVictory(out BattleResult outcome)
    {
        bool allEnemiesDead = _enemies.All(e => !e.IsAlive);
        bool allPlayersDead = _players.All(p => !p.IsAlive);

        if (allEnemiesDead)
        {
            outcome = BattleResult.Victory;
            return true;
        }
        if (allPlayersDead)
        {
            outcome = BattleResult.Defeat;
            return true;
        }
        outcome = BattleResult.None;
        return false;
    }

    // バトル終了処理：イベント通知と呼び出し元が更新しやすいよう最終プレイヤーステータスを返す
    private void EndBattle(BattleResult result)
    {
        //最終プレイヤーのHPを取得
        List<int> currentHps = _players.Select(p => p.CurrentHp).ToList();
        List<int> currentMps = _players.Select(p => p.CurrentMp).ToList();
        // 獲得経験値の計算
        int exp = _enemies.Sum(e => ((EnemyUnitData)e.Data).ExpPoint);
        // 結果の反映
        InventoryManager.Instance.UpdateFormedUnitState(currentHps,currentMps);
        InventoryManager.Instance.AddFormedUnitExp(exp);
        // ダンジョンマネージャーへの通知
        DungeonManager.Instance.HandleBattleFinish(result);

        // コルーチン停止と状態リセット
        if (_battleLoop != null) StopCoroutine(_battleLoop);
        _battleLoop = null;
        ChangeBattleState(BattleState.Idle);
    }
}


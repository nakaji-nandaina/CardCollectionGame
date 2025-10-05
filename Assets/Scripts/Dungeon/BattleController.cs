using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BattleState
{
    Idle,
    Preparing,
    Battle,
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

/// <summary>
/// 戦闘ユニットの情報を保持するクラス
/// </summary>
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
    public int Speed;
    public EffectResistances EffectResistances => Data.EffectResistances;
    public List<BaseSkillData> Skills => Data.Skills;
    public List<ActiveStatusEffect> ActiveStatusEffects = new List<ActiveStatusEffect>();
    public void AddStatusEffect(ActiveStatusEffect effect)
    {
        ActiveStatusEffects.Add(effect);
    }

    public UnitType Type;
    public bool IsAlive => CurrentHp > 0;
}

public class ActionNode : IComparable<ActionNode>
{
    public BattleUnit Unit;
    public float ActionTime;
    public string InstanceId => Unit.InstanceId+$"{ActionTime}";
    private int Sequence; // 同じActionTimeの場合の順序を保持するためのフィールド

    public int CompareTo(ActionNode other)
    {
        // ActionTimeの昇順
        // ActionTimeが同じ場合はSequenceの昇順
        if (ActionTime == other.ActionTime)
        {
            return Sequence.CompareTo(other.Sequence);
        }
        return ActionTime.CompareTo(other.ActionTime);
    }
    public ActionNode(BattleUnit unit, float actionTime, int sequence)
    {
        Unit = unit;
        ActionTime = actionTime;
        Sequence = sequence;
    }
}

public class BattleController : MonoBehaviour
{
    private float TurnDuration = 1.0f;
    private float ActionDuration = 0.5f;

    //戦闘中時間
    private int _baseActionInterval = 1000; //ベースの時間経過
    private float _actionTime = 0; //戦闘現在時刻
    private int _sequenceCounter = 0; // 同じActionTimeの場合の順序を保持するためのフィールド

    private int _lookaheadActions = 12;// 先読みする行動数(敵味方ユニットの総数*2以上)

    private BattleState _currentState = BattleState.Idle;

    public event Action<BattleState> OnBattleStateChanged;
    public event Action<List<BattleUnit>,List<BattleUnit>, List<ActionNode>> OnBattlePrepared;
    public event Action<BattleResult,int,List<BattleUnit>> OnBattleFinished; //バトルの終了を通知（結果、獲得経験値、最終プレイヤーステータス）
    public event Action<BattleUnit, BattleUnit, int> OnAttackPerformed;
    public event Action<List<ActionNode>> OnTurnStarted; // ターン開始を通知（行動者順リスト）

    public BattleState CurrentState => _currentState;

    private Coroutine _battleLoop;

    private List<BattleUnit> _players = new List<BattleUnit>();
    private List<BattleUnit> _enemies = new List<BattleUnit>();
    private PriorityQueue<ActionNode> _actionQueue = new PriorityQueue<ActionNode>();
    
    

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
        //Debug.Log($"Battle State changed to: {_currentState}");        
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
        _actionQueue.Clear();
        _actionTime = 0;
        // プレイヤーユニットの準備
        List<UnitSlot> playerSlot = InventoryManager.Instance.InBattleUnitList;
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
                Speed = slot.Speed,
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
            Speed = enemyUnitData.Speed,
            Type = UnitType.Enemy
        };
        _enemies.Add(battleUnit);
        FillActionQueueToLookahead();
        List<ActionNode> turnOrder = new List<ActionNode>(_actionQueue.Data);
        turnOrder.Sort();
        OnBattlePrepared?.Invoke(_players, _enemies, turnOrder);
        _battleLoop = StartCoroutine(BattleLoop());
    }

    // 先読みを時系列シミュレーションで埋める。
    private void FillActionQueueToLookahead()
    {
        // 生存ユニット一覧
        List<BattleUnit> allUnits = _players.Concat(_enemies).Where(u => u.IsAlive).ToList();
        if (allUnits.Count == 0) return;

        // シミュ用キュー
        PriorityQueue<ActionNode> sim = new PriorityQueue<ActionNode>();
        int simSeq = 0;

        // 既存キューの生存ノードをスナップショット（インスタンスを再利用して Sequence を保持）
        List<ActionNode> aliveExisting = _actionQueue.Data.Where(n => n.Unit.IsAlive).ToList();

        // いったんクリアして生きているノードだけ戻す
        _actionQueue.Clear();
        foreach (ActionNode e in aliveExisting) 
            _actionQueue.Push(e);

        // 各ユニットの最後の予定時刻を求める
        Dictionary<string, float> lastScheduled = new Dictionary<string, float>();
        foreach (ActionNode n in _actionQueue.Data)
        {
            // 同じユニットのより遅い予定があればそちらを優先
            if (!lastScheduled.TryGetValue(n.Unit.InstanceId, out var prev) || n.ActionTime > prev)
                lastScheduled[n.Unit.InstanceId] = n.ActionTime;
        }

        // キューに存在しないユニットは最低1回分を直接追加（遅いユニットが永久に欠けるのを防ぐ）
        foreach (var u in allUnits)
        {
            if (!lastScheduled.ContainsKey(u.InstanceId))
            {
                float firstTime = _actionTime + (float)_baseActionInterval / u.Speed;
                _actionQueue.Push(new ActionNode(u, firstTime, _sequenceCounter++));
                lastScheduled[u.InstanceId] = firstTime;
            }
        }

        // 各ユニットの「次の行動」を sim にシード
        foreach (var u in allUnits)
        {
            float start = lastScheduled.TryGetValue(u.InstanceId, out var last) ? Math.Max(_actionTime, last) : _actionTime;
            float next = start + (float)_baseActionInterval / u.Speed;
            sim.Push(new ActionNode(u, next, simSeq++));
        }

        // sim から取り出して実キューを埋める
        while (_actionQueue.Count < _lookaheadActions && sim.Count > 0)
        {
            ActionNode node = sim.Pop();
            _actionQueue.Push(new ActionNode(node.Unit, node.ActionTime, _sequenceCounter++));
            if (node.Unit.IsAlive)
            {
                float nextTime = node.ActionTime + (float)_baseActionInterval / node.Unit.Speed;
                sim.Push(new ActionNode(node.Unit, nextTime, simSeq++));
            }
        }
    }

    private IEnumerator BattleLoop()
    {
        ChangeBattleState(BattleState.Battle);
        while (true)
        {
            ActionNode nextAction = _actionQueue.Pop();
            BattleUnit unit = nextAction.Unit;
            _actionTime = (int)nextAction.ActionTime;

            //次の行動者が死亡していたらスキップ
            if (!unit.IsAlive)
                continue;
            //ターン順序リスト作成（現在行動するユニットを先頭に追加）
            List<ActionNode> turnOrder = new List<ActionNode> { nextAction };
            // 残りの行動キューを速度順にソートして追加
            List<ActionNode> queueCopy = new List<ActionNode> (_actionQueue.Data);
            queueCopy.Sort();
            turnOrder.AddRange(queueCopy);
            // ターン開始通知 
            OnTurnStarted?.Invoke(turnOrder);
            // ターン開始待機
            yield return new WaitForSeconds(TurnDuration);
            // ターン行動
            yield return PerformTurnAction(unit);
            // 勝敗判定
            if (CheckVictory(out var outcome)) 
            {
                ChangeBattleState(outcome == BattleResult.Victory ? BattleState.Victory : BattleState.Defeat);
                yield break; 
            }

            // 次の行動予定をキューに追加
            FillActionQueueToLookahead();
        }
    }

    /// <summary>
    /// ターン行動の実行
    /// 攻撃・スキル使用を状態に応じて実行
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    private IEnumerator PerformTurnAction(BattleUnit owner)
    {
        List<BattleUnit> ownerTeam = owner.Type == UnitType.Player ? _players : _enemies;
        List<BattleUnit> targetTeam = owner.Type == UnitType.Player ? _enemies : _players;
        
        if (targetTeam.TrueForAll(t => !t.IsAlive))
        {
            yield break; // 全てのターゲットが倒されている場合、攻撃を終了
        }
        List<BattleUnit> aliveTargetList = targetTeam.FindAll(t => t.IsAlive);
        BattleUnit target = aliveTargetList[UnityEngine.Random.Range(0, aliveTargetList.Count)];
        PerformAttackAction(owner, target);
        yield return new WaitForSeconds(ActionDuration);
    }

    private void PerformAttackAction(BattleUnit attacker, BattleUnit target)
    {
        int damage = Mathf.Max(0, attacker.AttackPoint - target.PhysDefensePoint);
        target.CurrentHp = Mathf.Max(0, target.CurrentHp - damage);
        OnAttackPerformed?.Invoke(attacker, target, damage);
        Debug.Log($"{attacker.Data.UnitName} attacks {target.Data.UnitName} for {damage} damage.");
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
        int exp = result == BattleResult.Victory ? _enemies.Sum(e => ((EnemyUnitData)e.Data).ExpPoint) : 0;
        // 結果の反映
        InventoryManager.Instance.UpdateBattleUnitState(currentHps,currentMps);
        InventoryManager.Instance.AddBattleUnitExp(exp);

        // ダンジョンマネージャーへの通知
        DungeonManager.Instance.HandleBattleFinish(result);

        // 終了イベントの発行
        OnBattleFinished?.Invoke(result, exp, _players);

        // コルーチン停止と状態リセット
        if (_battleLoop != null) StopCoroutine(_battleLoop);
        _battleLoop = null;
        ChangeBattleState(BattleState.Idle);
    }
}


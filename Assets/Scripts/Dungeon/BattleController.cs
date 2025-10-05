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
/// �퓬���j�b�g�̏���ێ�����N���X
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
    private int Sequence; // ����ActionTime�̏ꍇ�̏�����ێ����邽�߂̃t�B�[���h

    public int CompareTo(ActionNode other)
    {
        // ActionTime�̏���
        // ActionTime�������ꍇ��Sequence�̏���
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

    //�퓬������
    private int _baseActionInterval = 1000; //�x�[�X�̎��Ԍo��
    private float _actionTime = 0; //�퓬���ݎ���
    private int _sequenceCounter = 0; // ����ActionTime�̏ꍇ�̏�����ێ����邽�߂̃t�B�[���h

    private int _lookaheadActions = 12;// ��ǂ݂���s����(�G�������j�b�g�̑���*2�ȏ�)

    private BattleState _currentState = BattleState.Idle;

    public event Action<BattleState> OnBattleStateChanged;
    public event Action<List<BattleUnit>,List<BattleUnit>, List<ActionNode>> OnBattlePrepared;
    public event Action<BattleResult,int,List<BattleUnit>> OnBattleFinished; //�o�g���̏I����ʒm�i���ʁA�l���o���l�A�ŏI�v���C���[�X�e�[�^�X�j
    public event Action<BattleUnit, BattleUnit, int> OnAttackPerformed;
    public event Action<List<ActionNode>> OnTurnStarted; // �^�[���J�n��ʒm�i�s���ҏ����X�g�j

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
        // �v���C���[���j�b�g�̏���
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
        // �G���j�b�g�̏���
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

    // ��ǂ݂����n��V�~�����[�V�����Ŗ��߂�B
    private void FillActionQueueToLookahead()
    {
        // �������j�b�g�ꗗ
        List<BattleUnit> allUnits = _players.Concat(_enemies).Where(u => u.IsAlive).ToList();
        if (allUnits.Count == 0) return;

        // �V�~���p�L���[
        PriorityQueue<ActionNode> sim = new PriorityQueue<ActionNode>();
        int simSeq = 0;

        // �����L���[�̐����m�[�h���X�i�b�v�V���b�g�i�C���X�^���X���ė��p���� Sequence ��ێ��j
        List<ActionNode> aliveExisting = _actionQueue.Data.Where(n => n.Unit.IsAlive).ToList();

        // ��������N���A���Đ����Ă���m�[�h�����߂�
        _actionQueue.Clear();
        foreach (ActionNode e in aliveExisting) 
            _actionQueue.Push(e);

        // �e���j�b�g�̍Ō�̗\�莞�������߂�
        Dictionary<string, float> lastScheduled = new Dictionary<string, float>();
        foreach (ActionNode n in _actionQueue.Data)
        {
            // �������j�b�g�̂��x���\�肪����΂������D��
            if (!lastScheduled.TryGetValue(n.Unit.InstanceId, out var prev) || n.ActionTime > prev)
                lastScheduled[n.Unit.InstanceId] = n.ActionTime;
        }

        // �L���[�ɑ��݂��Ȃ����j�b�g�͍Œ�1�񕪂𒼐ڒǉ��i�x�����j�b�g���i�v�Ɍ�����̂�h���j
        foreach (var u in allUnits)
        {
            if (!lastScheduled.ContainsKey(u.InstanceId))
            {
                float firstTime = _actionTime + (float)_baseActionInterval / u.Speed;
                _actionQueue.Push(new ActionNode(u, firstTime, _sequenceCounter++));
                lastScheduled[u.InstanceId] = firstTime;
            }
        }

        // �e���j�b�g�́u���̍s���v�� sim �ɃV�[�h
        foreach (var u in allUnits)
        {
            float start = lastScheduled.TryGetValue(u.InstanceId, out var last) ? Math.Max(_actionTime, last) : _actionTime;
            float next = start + (float)_baseActionInterval / u.Speed;
            sim.Push(new ActionNode(u, next, simSeq++));
        }

        // sim ������o���Ď��L���[�𖄂߂�
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

            //���̍s���҂����S���Ă�����X�L�b�v
            if (!unit.IsAlive)
                continue;
            //�^�[���������X�g�쐬�i���ݍs�����郆�j�b�g��擪�ɒǉ��j
            List<ActionNode> turnOrder = new List<ActionNode> { nextAction };
            // �c��̍s���L���[�𑬓x���Ƀ\�[�g���Ēǉ�
            List<ActionNode> queueCopy = new List<ActionNode> (_actionQueue.Data);
            queueCopy.Sort();
            turnOrder.AddRange(queueCopy);
            // �^�[���J�n�ʒm 
            OnTurnStarted?.Invoke(turnOrder);
            // �^�[���J�n�ҋ@
            yield return new WaitForSeconds(TurnDuration);
            // �^�[���s��
            yield return PerformTurnAction(unit);
            // ���s����
            if (CheckVictory(out var outcome)) 
            {
                ChangeBattleState(outcome == BattleResult.Victory ? BattleState.Victory : BattleState.Defeat);
                yield break; 
            }

            // ���̍s���\����L���[�ɒǉ�
            FillActionQueueToLookahead();
        }
    }

    /// <summary>
    /// �^�[���s���̎��s
    /// �U���E�X�L���g�p����Ԃɉ����Ď��s
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    private IEnumerator PerformTurnAction(BattleUnit owner)
    {
        List<BattleUnit> ownerTeam = owner.Type == UnitType.Player ? _players : _enemies;
        List<BattleUnit> targetTeam = owner.Type == UnitType.Player ? _enemies : _players;
        
        if (targetTeam.TrueForAll(t => !t.IsAlive))
        {
            yield break; // �S�Ẵ^�[�Q�b�g���|����Ă���ꍇ�A�U�����I��
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

    // ���s����
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

    // �o�g���I�������F�C�x���g�ʒm�ƌĂяo�������X�V���₷���悤�ŏI�v���C���[�X�e�[�^�X��Ԃ�
    private void EndBattle(BattleResult result)
    {
        //�ŏI�v���C���[��HP���擾
        List<int> currentHps = _players.Select(p => p.CurrentHp).ToList();
        List<int> currentMps = _players.Select(p => p.CurrentMp).ToList();
        // �l���o���l�̌v�Z
        int exp = result == BattleResult.Victory ? _enemies.Sum(e => ((EnemyUnitData)e.Data).ExpPoint) : 0;
        // ���ʂ̔��f
        InventoryManager.Instance.UpdateBattleUnitState(currentHps,currentMps);
        InventoryManager.Instance.AddBattleUnitExp(exp);

        // �_���W�����}�l�[�W���[�ւ̒ʒm
        DungeonManager.Instance.HandleBattleFinish(result);

        // �I���C�x���g�̔��s
        OnBattleFinished?.Invoke(result, exp, _players);

        // �R���[�`����~�Ə�ԃ��Z�b�g
        if (_battleLoop != null) StopCoroutine(_battleLoop);
        _battleLoop = null;
        ChangeBattleState(BattleState.Idle);
    }
}


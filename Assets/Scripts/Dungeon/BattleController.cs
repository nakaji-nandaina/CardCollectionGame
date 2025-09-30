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

public class BattleController : MonoBehaviour
{
    private float TurnDuration = 1.0f;
    private float ActionDuration = 0.5f;

    //�퓬������
    private int _baseActionInterval = 1000; //�x�[�X�̎��Ԍo��
    private int _actionTime = 0; //�퓬���ݎ���
    private int _sequenceCounter = 0; // ����ActionTime�̏ꍇ�̏�����ێ����邽�߂̃t�B�[���h

    private BattleState _currentState = BattleState.Idle;

    public event Action<BattleState> OnBattleStateChanged;
    public event Action<List<BattleUnit>,List<BattleUnit>> OnBattlePrepared;
    public event Action<BattleResult,int,List<BattleUnit>> OnBattleFinished; //�o�g���̏I����ʒm�i���ʁA�l���o���l�A�ŏI�v���C���[�X�e�[�^�X�j
    public event Action<BattleUnit, BattleUnit, int> OnAttackPerformed;

    public BattleState CurrentState => _currentState;

    private Coroutine _battleLoop;

    private List<BattleUnit> _players = new List<BattleUnit>();
    private List<BattleUnit> _enemies = new List<BattleUnit>();
    private PriorityQueue<ActionNode> _actionQueue = new PriorityQueue<ActionNode>();
    private class ActionNode : IComparable<ActionNode>
    {
        public BattleUnit Unit;
        public float ActionTime;
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

        foreach(BattleUnit player in _players)
        {
            int nextTime = _actionTime + _baseActionInterval / player.Speed;
            ActionNode newNode = new ActionNode(player, nextTime, _sequenceCounter++);
            _actionQueue.Push(newNode);
        }

        foreach (BattleUnit enemy in _enemies)
        {
            int nextTime = _actionTime + _baseActionInterval / enemy.Speed;
            ActionNode newNode = new ActionNode(enemy, nextTime, _sequenceCounter++);
            _actionQueue.Push(newNode);
        }

        OnBattlePrepared?.Invoke(_players, _enemies);
        _battleLoop = StartCoroutine(BattleLoop());
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
            // �^�[���s��
            yield return PerformTurnAction(unit);
            // ���s����
            if (CheckVictory(out var outcome)) 
            {
                ChangeBattleState(outcome == BattleResult.Victory ? BattleState.Victory : BattleState.Defeat);
                yield break; 
            }

            // ���̍s�����Ԃ��v�Z���ăL���[�ɒǉ�
            int nextTime = _actionTime + _baseActionInterval / unit.Speed;
            ActionNode newNode = new ActionNode(unit, nextTime, _sequenceCounter++);
            _actionQueue.Push(newNode);
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
        int exp = _enemies.Sum(e => ((EnemyUnitData)e.Data).ExpPoint);
        // ���ʂ̔��f
        InventoryManager.Instance.UpdateFormedUnitState(currentHps,currentMps);
        InventoryManager.Instance.AddFormedUnitExp(exp);

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


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplorePresenter : MonoBehaviour
{
    [SerializeField, Header("�T���r���[")] private ExploreView _exploreView;
    [SerializeField, Header("�v���C���[���j�b�g�r���[")] private PlayerUnitsView _playerUnitsView;
    [SerializeField, Header("�^�[���p�l���r���[")] private TurnLaneView _turnPanelView;
    [SerializeField, Header("�o�g���R���g���[���[")] private BattleController _battleController;
    [SerializeField, Header("�o�g��UI�G�t�F�N�g�v���C���[")] private BattleUIfxPlayer _uifxPlayer;

    private readonly Dictionary<string, int> _enemyIndexById = new Dictionary<string, int>();

    private void OnEnable()
    {
        DungeonManager.Instance.OnDungeonStateChanged += OnDungeonStateChanged;

        _battleController.OnBattlePrepared += OnBattlePrepared;
        _battleController.OnBattleFinished += OnBattleFinished;
        _battleController.OnTurnStarted += OnTurnStarted;
        _battleController.OnAttackPerformed += OnAttackPerformed;
        // �_���W�����J�n�O�Ƀv���C���[���j�b�g�r���[��������
        _playerUnitsView.SetUp(InventoryManager.Instance.FormedUnitList);
    }
    private void Start()
    {
        DungeonManager.Instance.StartDungeon(GameSessionManager.Instance.SelectedDungeon);
    }

    private void OnDisable()
    {
        DungeonManager.Instance.OnDungeonStateChanged -= OnDungeonStateChanged;

        _battleController.OnBattlePrepared -= OnBattlePrepared;
        _battleController.OnBattleFinished -= OnBattleFinished;
        _battleController.OnTurnStarted -= OnTurnStarted;
        _battleController.OnAttackPerformed -= OnAttackPerformed;
    }

    private void OnDungeonStateChanged(DungeonState state)
    {
        switch (state)
        {
            case DungeonState.DungeonStart:
                InventoryManager.Instance.SetBattleUnit();
                // �_���W�����J�n���Ƀv���C���[���j�b�g�r���[��������
                _playerUnitsView.SetUp(InventoryManager.Instance.InBattleUnitList);
                AudioManager.Instance.PlayBGM(BGMName.Dungeon);
                break;
            case DungeonState.FloorStart:
                _exploreView.UpdateFloor(DungeonManager.Instance.CurrentFloor, OnUpdateFloorFinished);
                break;
            case DungeonState.FloorCleared:
                //_logView.AddLog("");
                //_logView.AddLog($"--- {DungeonManager.Instance.CurrentFloor}F�N���A ---");
                break;

        }
    }

    private void OnUpdateFloorFinished()
    {
        DungeonManager.Instance.NotifyFloorStartAnimationFinished();
    }

    private void OnBattlePrepared(List<BattleUnit> players, List<BattleUnit> enemies, List<ActionNode> nodes)
    {
        // ExploreView �ɓG�f�[�^��n���ď������i�\������ HP max ��ݒ�j
        if (enemies != null)
        {
            var enemyDatas = enemies.Select(e => (EnemyUnitData)e.Data).ToList();
            _exploreView.SetUp(enemyDatas);

            _exploreView.PlayEnemyEntranceAnimation();
            // �}�b�s���O�쐬�F�G�� InstanceId -> index
            _enemyIndexById.Clear();
            for (int i = 0; i < enemies.Count; i++)
            {
                _enemyIndexById[enemies[i].InstanceId] = i;
            }
        }

        // PlayerUnitView �� BattleUnit ���̌��ݒl�𔽉f
        if (_playerUnitsView != null && players != null)
        {
            _playerUnitsView.SetUp(players);
            foreach (var p in players)
            {
                Debug.Log($"Player Unit: {p.Data.UnitName}, HP: {p.CurrentHp}/{p.Data.MaxHealth}, MP: {p.CurrentMp}/{p.MaxMp}");
            }
            Debug.Log("PlayerUnitsView SetUp with players");
        }
        _turnPanelView.SetUp(nodes);
    }

    private void OnBattleFinished(BattleResult result, int exp, List<BattleUnit> players)
    {
        DungeonManager.Instance.NotifyFloorClearedAnimationFinished();
    }

    private void OnTurnStarted(List<ActionNode> nodes)
    {
        _turnPanelView.AnimateTurnOrder(nodes);
    }

    private void OnAttackPerformed(BattleUnit attacker, BattleUnit target, int damage)
    {
        //Attack1,2,3���烉���_���ɑI��
        SEName attackSE = (SEName)System.Enum.Parse(typeof(SEName), $"Attack{Random.Range(1, 4)}");
        AudioManager.Instance.PlaySE(SEName.Attack1);
        if (attacker.Type == UnitType.Player)
        {
            _playerUnitsView.Attack(attacker.InstanceId);
        }
        else if (attacker.Type == UnitType.Enemy)
        {
            if (_enemyIndexById.TryGetValue(target.InstanceId, out int idx))
            {
                _exploreView.PlayEnemyAttackAnimation(idx);
            }
        }


        if (target.Type == UnitType.Player)
        {
            _playerUnitsView.UpdateCurrent(target.InstanceId, target.CurrentHp, target.CurrentMp);
            // VFX�Đ��i�v���C���[���J�[�h�̏�j
            if (_playerUnitsView.TryGetRectTransform(target.InstanceId, out var rt))
            {
                var vfx = BattleUIfxType.Slash;
                _uifxPlayer.PlayAt(rt, vfx);
            }
            // ���S���Ă����玀�S�A�j���[�V����
            if (!target.IsAlive)
            {
                _playerUnitsView.Damaged(target.InstanceId);
            }
            // �����Ă���Ȃ�_���[�W�A�j���[�V����
            else
            {
                _playerUnitsView.Damaged(target.InstanceId);
            }
        }
        else if (target.Type == UnitType.Enemy)
        {
            if (_enemyIndexById.TryGetValue(target.InstanceId, out int idx))
            {
                _exploreView.UpdateEnemy(idx, target.CurrentHp);
                // VFX�Đ��i�G���j�b�g�̏�j
                if (_exploreView.TryGetEnemyRectTransform(idx, out RectTransform rt))
                {
                    var vfx = BattleUIfxType.Slash;
                    Vector3 pos = rt.position + new Vector3(0, 1f, 0);
                    _uifxPlayer.PlayAt(pos, vfx, size: 1.5f);
                }
                if (!target.IsAlive)
                {
                    // ���S�A�j���[�V����
                    _exploreView.PlayEnemyDeadAnimation(idx);
                }
                else
                {
                    _exploreView.PlayEnemyDamagedAnimation(idx);
                }
            }
        }
    }
}

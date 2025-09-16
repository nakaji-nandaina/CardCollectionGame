using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplorePresenter : MonoBehaviour
{
    [SerializeField, Header("�T���r���[")] private ExploreView _exploreView;
    [SerializeField, Header("�v���C���[���j�b�g�r���[")] private PlayerUnitsView _playerUnitsView;
    [SerializeField, Header("���O�r���[")] private LogView _logView;
    [SerializeField, Header("�o�g���R���g���[���[")] private BattleController _battleController;

    private readonly Dictionary<string, int> _enemyIndexById = new Dictionary<string, int>();

    private void OnEnable()
    {
        DungeonManager.Instance.OnDungeonStateChanged += OnDungeonStateChanged;

        _battleController.OnBattlePrepared += OnBattlePrepared;
        _battleController.OnAttackPerformed += OnAttackPerformed;

        _playerUnitsView.SetUp(InventoryManager.Instance.FormedUnitList);
        _logView.SetUp();
    }
    private void Start()
    {
        DungeonManager.Instance.StartDungeon(GameSessionManager.Instance.SelectedDungeon);
    }

    private void OnDisable()
    {
        if (DungeonManager.Instance != null)
        {
            DungeonManager.Instance.OnDungeonStateChanged -= OnDungeonStateChanged;
        }
        if (_battleController != null)
        {
            _battleController.OnBattlePrepared -= OnBattlePrepared;
            _battleController.OnAttackPerformed -= OnAttackPerformed;
        }
    }

    private void OnDungeonStateChanged(DungeonState state)
    {
        switch (state)
        {
            case DungeonState.DungeonStart:
                // �_���W�����J�n���Ƀv���C���[���j�b�g�r���[��������
                _playerUnitsView.SetUp(InventoryManager.Instance.FormedUnitList);
                AudioManager.Instance.PlayBGM(BGMName.Dungeon);
                break;
            case DungeonState.FloorStart:
                _playerUnitsView.SetUp(InventoryManager.Instance.FormedUnitList);
                _exploreView.UpdateFloor(DungeonManager.Instance.CurrentFloor);
                _logView.AddLog($"--- {DungeonManager.Instance.CurrentFloor}F���B ---");
                break;
            case DungeonState.FloorCleared:
                _logView.AddLog($"--- {DungeonManager.Instance.CurrentFloor}F�N���A ---");
                break;

        }
    }
    private void OnBattlePrepared(List<BattleController.BattleUnit> players, List<BattleController.BattleUnit> enemies)
    {
        _logView.AddLog("�퓬�J�n�I");
        // ExploreView �ɓG�f�[�^��n���ď������i�\������ HP max ��ݒ�j
        if (enemies != null)
        {
            var enemyDatas = enemies.Select(e => (EnemyUnitData)e.Data).ToList();
            _exploreView.SetUp(enemyDatas);

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
            _playerUnitsView.SetUp(InventoryManager.Instance.FormedUnitList);
            _playerUnitsView.SetUp(players);
        }
    }

    private void OnAttackPerformed(BattleController.BattleUnit attacker, BattleController.BattleUnit target, int damage)
    {
        _logView.AddLog($"{attacker.Data.UnitName} �̍U���I {target.Data.UnitName} �� {damage} �̃_���[�W�I");
        
        //Attack1,2,3���烉���_���ɑI��
        SEName attackSE = (SEName)System.Enum.Parse(typeof(SEName), $"Attack{Random.Range(1, 4)}");
        AudioManager.Instance.PlaySE(SEName.Attack1);
        if (attacker.Type == UnitType.Player)
        {
            _playerUnitsView.Attack(attacker.InstanceId);
        }


        if (target.Type == UnitType.Player)
        {
            _playerUnitsView.UpdateCurrent(target.InstanceId, target.CurrentHp, target.CurrentMp);
            _playerUnitsView.Damaged(target.InstanceId);
        }
        else if (target.Type == UnitType.Enemy)
        {
            if (_enemyIndexById.TryGetValue(target.InstanceId, out int idx))
            {
                _exploreView.UpdateEnemy(idx, target.CurrentHp);
            }
        }
    }
}

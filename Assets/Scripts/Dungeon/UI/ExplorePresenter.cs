using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplorePresenter : MonoBehaviour
{
    [SerializeField, Header("探索ビュー")] private ExploreView _exploreView;
    [SerializeField, Header("プレイヤーユニットビュー")] private PlayerUnitsView _playerUnitsView;
    [SerializeField, Header("ターンパネルビュー")] private TurnLaneView _turnPanelView;
    [SerializeField, Header("バトルコントローラー")] private BattleController _battleController;
    [SerializeField, Header("バトルUIエフェクトプレイヤー")] private BattleUIfxPlayer _uifxPlayer;

    private readonly Dictionary<string, int> _enemyIndexById = new Dictionary<string, int>();

    private void OnEnable()
    {
        DungeonManager.Instance.OnDungeonStateChanged += OnDungeonStateChanged;

        _battleController.OnBattlePrepared += OnBattlePrepared;
        _battleController.OnBattleFinished += OnBattleFinished;
        _battleController.OnTurnStarted += OnTurnStarted;
        _battleController.OnAttackPerformed += OnAttackPerformed;
        // ダンジョン開始前にプレイヤーユニットビューを初期化
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
                // ダンジョン開始時にプレイヤーユニットビューを初期化
                _playerUnitsView.SetUp(InventoryManager.Instance.InBattleUnitList);
                AudioManager.Instance.PlayBGM(BGMName.Dungeon);
                break;
            case DungeonState.FloorStart:
                _exploreView.UpdateFloor(DungeonManager.Instance.CurrentFloor, OnUpdateFloorFinished);
                break;
            case DungeonState.FloorCleared:
                //_logView.AddLog("");
                //_logView.AddLog($"--- {DungeonManager.Instance.CurrentFloor}Fクリア ---");
                break;

        }
    }

    private void OnUpdateFloorFinished()
    {
        DungeonManager.Instance.NotifyFloorStartAnimationFinished();
    }

    private void OnBattlePrepared(List<BattleUnit> players, List<BattleUnit> enemies, List<ActionNode> nodes)
    {
        // ExploreView に敵データを渡して初期化（表示順と HP max を設定）
        if (enemies != null)
        {
            var enemyDatas = enemies.Select(e => (EnemyUnitData)e.Data).ToList();
            _exploreView.SetUp(enemyDatas);

            _exploreView.PlayEnemyEntranceAnimation();
            // マッピング作成：敵の InstanceId -> index
            _enemyIndexById.Clear();
            for (int i = 0; i < enemies.Count; i++)
            {
                _enemyIndexById[enemies[i].InstanceId] = i;
            }
        }

        // PlayerUnitView に BattleUnit 側の現在値を反映
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
        //Attack1,2,3からランダムに選択
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
            // VFX再生（プレイヤー側カードの上）
            if (_playerUnitsView.TryGetRectTransform(target.InstanceId, out var rt))
            {
                var vfx = BattleUIfxType.Slash;
                _uifxPlayer.PlayAt(rt, vfx);
            }
            // 死亡していたら死亡アニメーション
            if (!target.IsAlive)
            {
                _playerUnitsView.Damaged(target.InstanceId);
            }
            // 生きているならダメージアニメーション
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
                // VFX再生（敵ユニットの上）
                if (_exploreView.TryGetEnemyRectTransform(idx, out RectTransform rt))
                {
                    var vfx = BattleUIfxType.Slash;
                    Vector3 pos = rt.position + new Vector3(0, 1f, 0);
                    _uifxPlayer.PlayAt(pos, vfx, size: 1.5f);
                }
                if (!target.IsAlive)
                {
                    // 死亡アニメーション
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

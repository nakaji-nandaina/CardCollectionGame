using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplorePresenter : MonoBehaviour
{
    [SerializeField, Header("探索ビュー")] private ExploreView _exploreView;
    [SerializeField, Header("プレイヤーユニットビュー")] private PlayerUnitsView _playerUnitsView;
    [SerializeField, Header("ログビュー")] private LogView _logView;
    [SerializeField, Header("バトルコントローラー")] private BattleController _battleController;

    private readonly Dictionary<string, int> _enemyIndexById = new Dictionary<string, int>();

    private void OnEnable()
    {
        DungeonManager.Instance.OnDungeonStateChanged += OnDungeonStateChanged;

        _battleController.OnBattlePrepared += OnBattlePrepared;
        _battleController.OnBattleFinished += OnBattleFinished;
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
                // ダンジョン開始時にプレイヤーユニットビューを初期化
                _playerUnitsView.SetUp(InventoryManager.Instance.FormedUnitList);
                AudioManager.Instance.PlayBGM(BGMName.Dungeon);
                break;
            case DungeonState.FloorStart:
                _playerUnitsView.SetUp(InventoryManager.Instance.FormedUnitList);
                _logView.AddLog("");
                _logView.AddLog($"--- {DungeonManager.Instance.CurrentFloor}F到達 ---");
                _exploreView.UpdateFloor(DungeonManager.Instance.CurrentFloor,OnUpdateFloorFinished);
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

    private void OnBattlePrepared(List<BattleController.BattleUnit> players, List<BattleController.BattleUnit> enemies)
    {
        _logView.AddLog("");
        _logView.AddLog("戦闘開始！");
        // ExploreView に敵データを渡して初期化（表示順と HP max を設定）
        if (enemies != null)
        {
            var enemyDatas = enemies.Select(e => (EnemyUnitData)e.Data).ToList();
            _exploreView.SetUp(enemyDatas);

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
            _playerUnitsView.SetUp(InventoryManager.Instance.FormedUnitList);
            _playerUnitsView.SetUp(players);
        }
    }

    private void OnBattleFinished(BattleResult result, int exp, List<BattleController.BattleUnit> players)
    {
        if (result == BattleResult.Victory)
        {
            _logView.AddLog("戦闘勝利！");
            _logView.AddLog($"{exp}exp 獲得");
        }
        else
        {
            _logView.AddLog("戦闘敗北...");
        }
        DungeonManager.Instance.NotifyFloorClearedAnimationFinished();
    }

    private void OnAttackPerformed(BattleController.BattleUnit attacker, BattleController.BattleUnit target, int damage)
    {
        _logView.AddLog($"{attacker.Data.UnitName} の攻撃！ {target.Data.UnitName} に {damage} のダメージ！");
        
        //Attack1,2,3からランダムに選択
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
                _exploreView.PlayEnemyDamagedAnimation(idx);
            }
        }
    }
}

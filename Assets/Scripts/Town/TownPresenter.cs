using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownPresenter : MonoBehaviour
{
    [SerializeField, Header("待ち画面")] private TownView _townView;

    private void OnEnable()
    {
        TownManager.OnTownChangeState += OnTownStateChanged;
        CheckStartUnit();
        _townView.SetUp(OnStartButtonPressed);
    }

    private void OnDisable()
    {
        TownManager.OnTownChangeState -= OnTownStateChanged;
    }

    private void OnTownStateChanged(TownState state)
    {
        switch (state)
        {
            case TownState.Idle:
                AudioManager.Instance.PlayBGM(BGMName.Town);
                Debug.Log("拠点に到着");
                break;

            case TownState.Start:
                StartDungeon();
                break;
        }
    }

    private void CheckStartUnit()
    {
        // チュートリアル用に最初の5体を編成に追加
        if (InventoryManager.Instance.FormedUnitList.Count == 0)
        {
            for(int i = 0; i < 5; i++)
            {
                UnitSlot slot = new UnitSlot(Database.Instance.PlayerUnitDatabase[i]);
                InventoryManager.Instance.AddFormedUnit(slot);
            }
        }
    }

    private void OnStartButtonPressed()
    {
        TownManager.Instance.ChangeState(TownState.Start);
    }

    private void StartDungeon()
    {
        GameSessionManager.Instance.SetSelectedDungeon(Database.Instance.DungeonDatabase[0]);
        SceneLoader.LoadScene(SceneName.Dungeon);
    }

}

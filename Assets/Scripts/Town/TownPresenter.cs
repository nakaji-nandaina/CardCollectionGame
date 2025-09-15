using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownPresenter : MonoBehaviour
{
    [SerializeField, Header("‘Ò‚¿‰æ–Ê")] private TownView _townView;

    private void OnEnable()
    {
        TownManager.OnTownChangeState += OnTownStateChanged;
        CheckStartUnit();
        _townView.SetUp(OnStartButtonPressed);
    }
    private void OnTownStateChanged(TownState state)
    {
        switch (state)
        {
            case TownState.Idle:
                break;

            case TownState.Start:
                StartDungeon();
                break;
        }
    }

    private void CheckStartUnit()
    {
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

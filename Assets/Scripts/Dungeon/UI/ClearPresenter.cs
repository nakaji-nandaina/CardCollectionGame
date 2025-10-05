using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPresenter : MonoBehaviour
{
    [SerializeField] ClearPanelView _clearPanelView;
    [SerializeField] PanelViewBase _blockPanelView;
 
    private void OnEnable()
    {
        DungeonManager.Instance.OnDungeonStateChanged += OnDungeonStateChanged;
    }

    private void OnDisable()
    {
        DungeonManager.Instance.OnDungeonStateChanged -= OnDungeonStateChanged;
    }

    private void OnDungeonStateChanged(DungeonState state)
    {
        if(state == DungeonState.DungeonCleared)
        {
            OnClear();
        }
    }

    private void OnClear()
    {
        _clearPanelView.SetUp(Retry,ToTown);
        _clearPanelView.Show();
        _blockPanelView.Show();
    }

    // 再挑戦
    private void Retry()
    {
        //ユニットの経験値を確定する
        InventoryManager.Instance.SetFormedUnit();

        SceneLoader.ReLoadScene();
    }

    // 拠点へ戻る
    private void ToTown()
    {
        //ユニットの経験値を確定する
        InventoryManager.Instance.SetFormedUnit();

        SceneLoader.LoadScene(SceneName.Town);
    }
}

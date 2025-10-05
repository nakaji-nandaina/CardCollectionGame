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

    // �Ē���
    private void Retry()
    {
        //���j�b�g�̌o���l���m�肷��
        InventoryManager.Instance.SetFormedUnit();

        SceneLoader.ReLoadScene();
    }

    // ���_�֖߂�
    private void ToTown()
    {
        //���j�b�g�̌o���l���m�肷��
        InventoryManager.Instance.SetFormedUnit();

        SceneLoader.LoadScene(SceneName.Town);
    }
}

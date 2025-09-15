using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database Instance { get; private set; }
    [SerializeField, Header("�v���C���[���j�b�g�f�[�^�x�[�X")]private List<PlayerUnitData> _playerUnitDatabase;
    [SerializeField, Header("�_���W�����f�[�^�x�[�X")] private List<DungeonData> _dungeonDatabase;

    public List<PlayerUnitData> PlayerUnitDatabase => _playerUnitDatabase;
    public List<DungeonData> DungeonDatabase => _dungeonDatabase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}

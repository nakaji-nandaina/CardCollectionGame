using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonData", menuName = "DungeonData")]
public class DungeonData : BaseScriptableObject
{
    [SerializeField, Header("�ő�K�w")] private int _maxFloor = 10;
    [SerializeField, Header("�w�i�摜")] private Sprite _backgroundImage;
    [SerializeField, Header("�_���W������")] private string _dungeonName = "Dungeon";
    [SerializeField, Header("BGM")] private AudioClip _bgm = null;
    [SerializeField, Header("�G���")] private List<EnemyUnitData> _enemyUnitDatas = new List<EnemyUnitData>();
    [SerializeField, Header("�{�X���")] private EnemyUnitData _bossUnitData = null;

    public int MaxFloor => _maxFloor;
    public Sprite BackgroundImage => _backgroundImage;
    public string DungeonName => _dungeonName;
    public AudioClip BGM => _bgm;
    public List<EnemyUnitData> EnemyUnitDatas => _enemyUnitDatas;
    public EnemyUnitData BossUnitData => _bossUnitData;
    public EnemyUnitData GetRandomEnemy => _enemyUnitDatas[Random.Range(0, _enemyUnitDatas.Count)];
}

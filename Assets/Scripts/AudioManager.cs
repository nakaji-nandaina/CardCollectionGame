using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SEName
{
    Attack1,
    Attack2,
    Attack3,
    Magic,
    Damaged,
    Heal,
    Clear,
    LevelUp,
}

[System.Serializable]
public enum BGMName
{
    Title,
    Town,
    Dungeon,
    Boss,
    Victory,
}

/// <summary>
/// BGMの名前とAudioClipのペア
/// </summary>
[System.Serializable]
public class BGMEntry
{
    public BGMName Name;
    public AudioClip Clip;
}

/// <summary>
/// SEの名前とAudioClipのペア
/// </summary>
[System.Serializable]
public class SEEntry
{
    public SEName Name;
    public AudioClip Clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField, Header("BGM用")]private AudioSource _bgmSource;
    [SerializeField, Header("SE用")]private AudioSource _seSource;

    [SerializeField, Header("BGMリスト")] private List<BGMEntry> _bgmList;
    [SerializeField, Header("SEリスト")] private List<SEEntry> _seList;

    private Dictionary<BGMName, AudioClip> _bgmDictionary = new Dictionary<BGMName, AudioClip>();
    private Dictionary<SEName, AudioClip> _seDictionary = new Dictionary<SEName, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _bgmSource.loop = true;
            InitDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitDictionary()
    {
        _bgmDictionary.Clear();
        _seDictionary.Clear();
        foreach (BGMEntry entry in _bgmList) 
        {
            _bgmDictionary.Add(entry.Name, entry.Clip);
        }
        foreach (SEEntry entry in _seList)
        {
            _seDictionary.Add(entry.Name, entry.Clip);
        }
    }

    public void PlayBGM(BGMName name)
    {
        if (!_bgmDictionary.TryGetValue(name, out AudioClip clip))
        {
            throw new KeyNotFoundException($"BGM '{name}' not found in dictionary.");
        }
        if (_bgmSource.clip == clip && _bgmSource.isPlaying)
        {
            return; // 同じBGMが再生中なら何もしない
        }
        _bgmSource.clip = clip;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
        _bgmSource.clip = null;
    }

    public void PlaySE(SEName name)
    {
        if (!_seDictionary.TryGetValue(name, out AudioClip clip))
        {
            throw new KeyNotFoundException($"SE '{name}' not found in dictionary.");
        }
        _seSource.PlayOneShot(clip);
    }

    public void StopSE()
    {
        _seSource.Stop();
    }
}


using System;
using UnityEngine;

[Serializable]
public class GameLevel
{
    public bool locked;
    public string scene;
    public string name;
    public string description;
    public Sprite image;
    
    public int coins { get; set; }
    public float time { get; set; }
    public static readonly int starsLevel = 3;
    public bool[] stars { get; set; } = new bool[starsLevel];

    public virtual void LoadState(LevelData levelData)
    {
        locked = levelData.locked;
        coins = levelData.coins;
        time = levelData.time;
        stars = levelData.stars;
    }
    
    public static string FormattedTime(float time)
    {
        var minutes = Mathf.FloorToInt(time / 60f);
        var seconds = Mathf.FloorToInt(time % 60f);
        var milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        return minutes.ToString("0") + "'" + seconds.ToString("00") + "\"" + milliseconds.ToString("00");
    }
}
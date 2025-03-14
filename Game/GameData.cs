using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameData
{
    public int retries;
    public LevelData[] levels;
    public string createdAt;
    public string updatedAt;
    
    public virtual string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static GameData FromJson(string json)
    {
        return JsonUtility.FromJson<GameData>(json);
    }

    public static GameData Create()
    {
        return new GameData()
        {
            retries = Game.Instance.retries,
            createdAt = DateTime.UtcNow.ToString(),
            updatedAt = DateTime.UtcNow.ToString(),
            levels = Game.Instance.levels.Select((level) =>
            {
                return new LevelData()
                {
                    locked = level.locked
                };
            }).ToArray()
        };
    }
}
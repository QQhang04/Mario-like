using System;
using UnityEngine;

[Serializable]
public class GameLevel
{
    public bool islocked;
    public string scene;
    public string name;
    public string description;
    public Sprite image;
    
    public int coins { get; set; }
    public float time { get; set; }
    public static readonly int starsLevel = 3;
    public bool[] stars { get; set; } = new bool[starsLevel];
}
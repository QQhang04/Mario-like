using System;

[Serializable]
public class LevelData
{
    public bool locked;
    public int coins;
    public float time;
    public bool[] stars = new bool[GameLevel.starsLevel];
}
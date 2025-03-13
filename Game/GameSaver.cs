using System;

public class GameSaver : Singleton<GameSaver>
{
    protected static readonly int TotalSlots = 5;
    public virtual GameData[] loadList()
    {
        var list = new GameData[TotalSlots];
        for (int i = 0; i < TotalSlots; i++)
        {
            var data = Load(i);

            if (data)
            {
                list[i] = data;
            }
        }

        return list;
    }
}
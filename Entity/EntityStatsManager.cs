using System;
using UnityEngine;

public abstract class EntityStatsManager<T> : MonoBehaviour where T : EntityStats<T>
{
    public T[] stats;
    public T current { get; private set; }

    public virtual void Change(int to)
    {
        if (to >= 0 && to < stats.Length)
        {
            if (current != stats[to])
            {
                current = stats[to];
            }
        }
    }

    protected void Start()
    {
        if (stats.Length > 0)
        {
            current = stats[0];
        }
    }
}
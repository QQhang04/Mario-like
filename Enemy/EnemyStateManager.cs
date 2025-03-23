using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyStateManager : EntityStateManager<Enemy>
{
    [ClassTypeName(typeof(EnemyState))]
    public string[] states;

    protected override List<EntityState<Enemy>> GetStateList()
    {
        return EnemyState.CreateListFromStringArray(states);
    }
}


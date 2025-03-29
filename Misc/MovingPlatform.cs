using System;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(WaypointManager))]
public class MovingPlatform : MonoBehaviour
{
    public float moveSpeed = 5f;
    private WaypointManager waypoints;
    protected virtual void Initialize()
    {
        waypoints = GetComponent<WaypointManager>();
        tag = GameTag.Platform;
    }

    protected virtual void Awake()
    {
        Initialize();
    }

    protected void Update()
    {
        var pos = transform.position;
        var targetPos = waypoints.current.position;
        pos = Vector3.MoveTowards(pos, targetPos, Time.deltaTime * moveSpeed);
        
        transform.position = pos;

        if (Vector3.Distance(pos, targetPos) < 0.1f)
        {
            waypoints.Next();
        }
    }
}
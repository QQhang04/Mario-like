using UnityEngine;

public class PlayerStats : EntityStats<PlayerStats>
{
    [Header("General")] 
    public float rotationSpeed = 970f;
    [Header("Motion Stats")] 
    public float brakeThreshold = -.8f;
    public float turningDrag = 28f;
    public float acceleration = 13f;
    public float topSpeed = 6f;
    public float airAcceleration = 32f;
    public float deceleration = 28f;
    
    [Header("Running Stats")]
    public float runningAcceleration = 16f;
    public float runningTopSpeed = 7.5f;
    public float runningTurningDrag = 14f;

    // [Header("Backflip Stats")] 
    // public bool canBackflip = true;

}
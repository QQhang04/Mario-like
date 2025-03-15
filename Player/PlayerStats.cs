using UnityEngine;

public class PlayerStats : EntityStats<PlayerStats>
{
    [Header("General")] 
    public float rotationSpeed = 970f;
    public float friction = 16f;
    public float gravityTopSpeed = 50f;
    public float gravity = 38f;
    public float fallGravity = 65f;
    public float pushForce = 4f;
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

    [Header("Backflip Stats")] 
    public bool canBackflip = true;
    public float backflipJumpHeight = 23f;
    public float backflipGravity = 35f;
    public float backflipTurningDrag = 2.5f;
    public float backflipAirAcceleration = 12f;
    public float backflipTopSpeed = 7.5f;
    public float backflipBackwardTurnForce = 8f;

    [Header("Jump Stats")]
    public int multiJumps = 3;
    public float coyoteJumpThreshold = .15f;
    public float maxJumpHeight = 17f;
    public float minJumpHeight = 1f;

    [Header("Stomp Attack Stats")] 
    public bool canStompAttack = true;
    public float stompAirTime = .8f;
    public float stompDownwardForce = 20f;
    public float stompGroundTime = .5f;
    public float stompGroundLeapHeight = 10f;
}
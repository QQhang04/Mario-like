using UnityEngine;

public class PlayerStats : EntityStats<PlayerStats>
{
    [Header("General stats")] 
    public float rotationSpeed = 970f;
    public float friction = 16f;
    public float gravityTopSpeed = 50f;
    public float gravity = 38f;
    public float fallGravity = 65f;
    public float pushForce = 4f;
    public float snapForce = 15f;
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
    
    [Header("Hurt Stats")]
    public float hurtUpwardForce = 10f;
    public float hurtBackwardsForce = 5f;
    
    [Header("Spin Stats")]
    public bool canSpin = true;
    public bool canAirSpin = true;
    public float spinDuration = 0.5f;
    public float airSpinUpwardForce = 10f;
    public int allowedAirSpins = 1;
    
    [Header("Pick and Throw Stats")]
    public bool canPickUp = true;
    public bool canPickUpOnAir = false;
    public bool canJumpWhileHolding = true;
    public float pickDistance = 0.5f;
    public float throwVelocityMultiplier = 1.5f;
}
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
    public float slopeUpwardForce = 25f;
    public float slopeDownwardForce = 28f;
    
    [Header("Running Stats")]
    public float runningAcceleration = 16f;
    public float runningTopSpeed = 7.5f;
    public float runningTurningDrag = 14f;
    
    [Header("Ledge Hanging Stats")]
    public bool canLedgeHang = true;
    public LayerMask ledgeHangingLayers;
    public Vector3 ledgeHangingSkinOffset;
    public float ledgeMaxForwardDistance = 0.25f;
    public float ledgeMaxDownwardDistance = 0.25f;
    public float ledgeSideMaxDistance = 0.5f;
    public float ledgeSideHeightOffset = 0.15f;
    public float ledgeSideCollisionRadius = 0.25f;
    public float ledgeMovementSpeed = 1.5f;

    [Header("Ledge Climbing Stats")]
    public bool canClimbLedges = true;
    public LayerMask ledgeClimbingLayers;
    public Vector3 ledgeClimbingSkinOffset;
    public float ledgeClimbingDuration = 1f;

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
    
    [Header("Swimming Stats")]
    public float waterConversion = 0.35f;
    public float waterRotationSpeed = 360f;
    public float waterUpwardsForce = 8f;
    public float waterJumpHeight = 15f;
    public float waterTurningDrag = 2.5f;
    public float swimAcceleration = 4f;
    public float swimDeceleration = 3f;
    public float swimTopSpeed = 4f;
    public float swimDiveForce = 15f;
    public float buoyancyFactor = .5f;
    
    [Header("Rail Grinding Stats")]
    public bool useCustomCollision = true;
    public float grindRadiusOffset = 0.26f;
    public float minGrindInitialSpeed = 10f;
    public float minGrindSpeed = 5f;
    public float grindTopSpeed = 25f;
    public float grindDownSlopeForce = 40f;
    public float grindUpSlopeForce = 30f;

    [Header("Rail Grinding Brake")]
    public bool canGrindBrake = true;
    public float grindBrakeDeceleration = 10;

    [Header("Rail Grinding Dash Stats")]
    public bool canGrindDash = true;
    public bool applyGrindingSlopeFactor = true;
    public float grindDashCoolDown = 0.5f;
    public float grindDashForce = 25f;
}
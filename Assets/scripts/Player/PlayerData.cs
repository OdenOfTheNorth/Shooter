using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New PlayerData", menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    public int MaxJumpCount = 1;
    public float WalkSpeed = 10f;
    public float RunSpeed = 10f;
    [Header("Gravity")]
    public float gravity = 10f;
    public float GravityIncrease = 5f;
    public float MaxGravity = 40f;
    [Header("Jumping and Air Movement")]
    public float jumpStrength = 10f;
    public float WallJumpDirStrength = 10f;
    public float WallJumpUpStrength = 10f;
    [Range(0.0f, 1.0f)]
    public float AirControll = 10;
    public float coyoteTime = 0.5f;
    [Header("Drag")]
    public float groundDrag = 6f;
    public float airDrag = 2f;
    public float grappelDrag = 0f;
    public float slideDrag = 0.2f;
    [Header("Roation and Acceliration speed")]
    public float runAcceliration = 10f;
    public float RotationSpeed = 10f;
    
}

//[Header("CounterForce")]
//public float MaxCounterForce = 15 * 15;
//public float MinCounterForce = 9 * 9;
/*
public float MaxSpeed = 35;
public float GroundCounterForce = 10;
public float SlideCounterForce = 10;
public float AirCounterForce = 10;*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerData", menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    public int MaxJumpCount = 1;
    public float WalkSpeed = 10f;
    public float RunSpeed = 10f;
    public float gravity = 10f;
    public float jumpStrength = 10f;
    public float WallJumpStrength = 10f;
    [Range(0.0f, 1.0f)]
    public float AirControll = 10;
    [Header("CounterForce")]
    public float MaxCounterForce = 15 * 15;
    public float MinCounterForce = 9 * 9;
    
    public float GroundCounterForce = 10;
    public float SlideCounterForce = 10;
    public float AirCounterForce = 10;
    public float RotationSpeed = 10f;
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public PlayerMovement playerMovement;

    [Range(0, 1)] public float maxAngle = 1;
    [Range(-1, 0)] public float minAngle = -1;
    
    private Vector3 WallNormal;
    private Vector3 lastWallNormal;
    public LayerMask wallLayer;
    
    public void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Update()
    {
        if (playerMovement.OnWall)
        {
            Vector3 antiGravity = playerMovement.GravityDirection * -playerMovement.playerData.gravity;
            Vector3 forcePlayerIntoWall = Vector3.zero;//WallNormal * -playerMovement.playerData.gravity;
            Vector3 forwardWallVector = playerMovement.forwardWallVector;
            
            playerMovement.rigidbody.AddForce(antiGravity + forcePlayerIntoWall + forwardWallVector, ForceMode.Force);
            
            if (playerMovement.jumpInput)
            {
                Vector3 wallJump = (WallNormal * playerMovement.playerData.WallJumpStrength) - (playerMovement.GravityDirection * playerMovement.playerData.jumpStrength);//+ ( * -playerData.jumpStrength);
                playerMovement.rigidbody.AddForce(wallJump, ForceMode.Impulse);
                EndWallRun();
            }
        }
        //CheckWall();
    }
    
    public bool CheckWall()
    {
        Vector3 pos = playerMovement.cachedtransform.position;
        Vector3 right = playerMovement.cachedtransform.right;
        Vector3 forward = playerMovement.cachedtransform.forward;
        
        Vector3[] directions =
        {
            //forward,
            right,
            //(right + forward).normalized,
            //(right - forward).normalized,
            //-forward,
            -right,
            //(-right + forward).normalized,
            //(-right - forward).normalized
        };

        RaycastHit[] hit = new RaycastHit[directions.Length]; 
        
        //int layerMask = 1 << WallLayer;
        //layerMask = ~layerMask;
        
        for (int i = 0; i < directions.Length; i++)
        {
            Physics.Raycast(pos, directions[i], out hit[i], playerMovement.maxWallDistance, playerMovement.layerMask);
            Debug.DrawRay(pos, directions[i] * playerMovement.maxWallDistance, Color.red);
           
            if (hit[i].collider != null)
            {
                //print("OnWall");
                
                StartWallRun(hit[i].normal);
                return true;
            }
        }
        EndWallRun();
        return false;
    }
    
    public void OnCollisionEnter(Collision other)
    {
        StartWallRun(other.contacts[0].normal);    
        
        if (other.collider.gameObject.layer == 1<<wallLayer)
        {
            
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (playerMovement.OnWall)
        {
            EndWallRun();
        }
    }

    public void StartWallRun(Vector3 WallNormal)
    {
        if (WallNormal == lastWallNormal)
        {
            return;
        }
        
        if ((playerMovement.forwardInput == 0 || playerMovement.rightInput == 0) && playerMovement.OnGround)
        {
            return;
        }

        float dotAngle = Vector3.Dot(playerMovement.forwardVector, WallNormal);

        print(dotAngle);

        if (dotAngle < maxAngle && dotAngle > minAngle)
        {
            lastWallNormal = WallNormal;
            playerMovement.OnWall = true;
            playerMovement.forwardWallVector = Vector3.ProjectOnPlane(playerMovement.forwardVector, WallNormal).normalized * playerMovement.currentSpeed;
        }
    }
    
    public void EndWallRun()
    {   
        playerMovement.ShouldApplyGravity = true;
        playerMovement.OnWall = false;
    }

    void ResetJump()
    {
        playerMovement.CurrnetJumpCount = 1;
    }
}

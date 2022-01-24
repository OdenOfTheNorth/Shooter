using System;
using UnityEngine;



public class GravityRamp : MonoBehaviour
{
    public Transform middlePoint;
    public Transform playerTrans;
    public PlayerMovement playerMovement;
    public Vector3 dir;
    public Vector3 lastDir;
    public Vector3 lastPos;
    public Vector3 NewDir;
    public Vector3 counterDir;
    public Vector3 gravityVector;
    private bool useOnTrigger;
    public bool ReverseVector = true;
    public bool draw = true;

    public enum Direction
    {
        Forward,
        Right,
        Up
    }
    
    public Direction direction;


    private void Start()
    {
        if (!middlePoint)
        {
            middlePoint = transform;
        }
    }

    void Update()
    {
        if (playerMovement)
        {
            Vector3 directionVector = Vector3.zero;

            switch (direction)
            {
                case Direction.Forward:
                    directionVector = transform.forward;
                    break;
                case Direction.Right:
                    directionVector = transform.right;
                    break;
                case Direction.Up:
                    directionVector = transform.up;
                    break;
            }
            
            dir = (playerTrans.position - middlePoint.position);

            counterDir = Vector3.Dot(dir,directionVector) * directionVector;

            gravityVector = counterDir - dir;

            if (playerMovement)
            {
                playerMovement.GravityDirection = ReverseVector ? -gravityVector : gravityVector;    
            }
        }
        
        //Debug.DrawLine(middlePoint.position, lastPos);
    }

    private void OnTriggerStay(Collider other)
    {
        playerMovement = other.GetComponent<PlayerMovement>();
        
        if (playerMovement)
        {
            playerTrans = other.transform;
            lastDir = playerMovement.GravityDirection;
        }
    }

    private void OnTriggerExit(Collider other)
    {


        NewDir = ReverseVector ? -gravityVector : gravityVector;
        /*
        NewDir.x = Mathf.RoundToInt(dir.x);
        NewDir.y = Mathf.RoundToInt(dir.y);
        NewDir.z = Mathf.RoundToInt(dir.z);

        if (NewDir.x > 1)
        {
            NewDir.x = NewDir.x / NewDir.x;
        }
        
        if (NewDir.y > 1)
        {
            NewDir.y = NewDir.y / NewDir.y;
        }
        
        if (NewDir.z > 1)
        {
            NewDir.z = NewDir.z / NewDir.z;
        }

        print("X old = " + dir.x + " X new = " + NewDir.x);
        print("Y old = " + dir.y + " Y new = " + NewDir.y);
        print("Z old = " + dir.z + " Z new = " + NewDir.z);

        print(NewDir);
        */
        NewDir.Normalize();
        
        if (!playerMovement)
        {
            playerMovement = other.GetComponent<PlayerMovement>();    
        }
        
        if (playerMovement)
        {
            playerMovement.GravityDirection = NewDir;
            playerMovement = null;
        }
    }
    private void OnDrawGizmos()
    {
        if (!draw)
        {
            return;
        }
        
        if (playerMovement)
        {
            Gizmos.color = Color.black;    
            Gizmos.DrawLine(middlePoint.position, playerMovement.orientation.position);
        }

        if (middlePoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(middlePoint.position, middlePoint.position + counterDir);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(middlePoint.position, middlePoint.position + dir);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(middlePoint.position, middlePoint.position - gravityVector);
        }
    }
}

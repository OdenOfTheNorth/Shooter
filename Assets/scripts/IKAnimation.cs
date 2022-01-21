using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKAnimation : MonoBehaviour
{
    public float MaxDistance = 1;
    public float sideDistance = 1;

    private Transform trans;
    
    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = trans.position;
        Vector3 rightStart = pos + (trans.right * sideDistance);
        Vector3 leftStart = pos + (trans.right * -sideDistance);

        RaycastHit leftHit;
        RaycastHit rightHit;
        
        Physics.Raycast(leftStart, Vector3.down, out leftHit, 100);
        Physics.Raycast(rightStart, Vector3.down, out rightHit, 100);

        if (leftHit.collider != null)
        {
            Debug.DrawLine(leftHit.point, leftStart, Color.red);
            print(leftHit.point);
            
            if (leftHit.point.sqrMagnitude < MaxDistance * MaxDistance)
            {
                
            }
        }
        else
        {
            print("left Leg did not hit");
        }
        
        if (rightHit.collider != null)
        {
            Debug.DrawLine(rightHit.point, rightStart, Color.red);
            print(rightHit.point);
            if (rightHit.point.sqrMagnitude < MaxDistance * MaxDistance)
            {
                
            }
        }
        else
        {
            print("right leg did not hit");
        }
    }
}

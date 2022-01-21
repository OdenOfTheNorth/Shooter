using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ParentAndChild
{
    public ParentAndChild(Transform parent,Transform child)
    {
        Parent = parent;
        Child = child;
    }
    
    public Transform Parent;
    public Transform Child;
}

public class Platform : MonoBehaviour
{
    public bool RotateObject = false;
    public float RotSpeed = 15;

    public bool MoveObject = false;
    public Transform trans;
    public Transform[] targets;
    public int targetNumber = 0;
    private Transform current_target;
    public float MoveSpeed = 15;
    public float tolarance = 15;

    private Vector3 Direction;
    
    public List<ParentAndChild> parentAndChild = new List<ParentAndChild>(10);
    public int Listcount = 0;

    void Start()
    {
        trans = transform;
        
        if (targets.Length != 0)
        {
            if (targets.Length > 0)
            {
                current_target = targets[0];
            }
        }
    }

    void FixedUpdate()
    {
        Listcount = parentAndChild.Count;

        if (RotateObject)
        {
            Vector3 Rot = trans.rotation.eulerAngles;

            Rot.y += RotSpeed * Time.deltaTime;
        
            trans.rotation = Quaternion.Euler(Rot); 
        }
        
        if (MoveObject)
        {
            if (targets.Length > 0)
            {
                if (trans.position != current_target.position)
                {
                    MovePlatform();
                }
                else
                {
                    UpdatePlatform();
                }
            }
        }
    }

    void MovePlatform()
    {
        current_target = targets[targetNumber];

        Vector3 pos = trans.position;
        
        Direction = (current_target.position - pos);
        trans.position += (Direction.normalized) * MoveSpeed * Time.fixedDeltaTime;
        
        if (Direction.magnitude < tolarance)
        {
            NextPlatform();
        }
    }
    
    void UpdatePlatform()
    {
        NextPlatform();
    }
    
    void NextPlatform()
    {
        targetNumber++;
        if (targetNumber >= targets.Length)
        {
            targetNumber = 0;
        }

        current_target = targets[targetNumber];
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        Rigidbody Rigidbody = other.GetComponent<Rigidbody>();
        
        if (Rigidbody)
        {
            print(other.name);
            
            Transform parent = other.transform.parent;
            Transform obj = other.transform;
            
            for (int i = 0; i < parentAndChild.Count; i++)
            {
                if (parentAndChild[i].Child != null || parentAndChild[i].Parent != null)
                {
                    continue;
                }
                
                parentAndChild[i] = new ParentAndChild(parent, obj);
                other.transform.parent = gameObject.transform;
                print(other.name);
                return;
            }
            
            parentAndChild.Add(new ParentAndChild(parent, obj));
            other.transform.parent = gameObject.transform;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Rigidbody Rigidbody = other.GetComponent<Rigidbody>();

        if (Rigidbody)
        {
            Transform parent = other.transform.parent;
            Transform obj = other.transform;

            for (int i = 0; i < parentAndChild.Count; i++)
            {
                if (parentAndChild[i].Child == obj)
                {
                    obj.parent = parentAndChild[i].Parent;
                    parentAndChild[i] = new ParentAndChild(null, null);
                }
            }
            print(other.name);
        }
    }
}


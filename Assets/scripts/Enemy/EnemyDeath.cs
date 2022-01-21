using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : ActionToDo
{
    public GameObject AudioSource;
    public GameObject ExplationEffect;
    
    public override void Call()
    {
        Instantiate(AudioSource, transform.position, Quaternion.identity);
        Instantiate(ExplationEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

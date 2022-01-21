using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : ActionToDo
{
    //public string level = "";
    public float DeathDuration = 2.0f;
    
    public override void Call()
    {
        StartCoroutine(Dead());
    }
    
    public IEnumerator Dead()
    {
        Rigidbody body = GetComponent<Rigidbody>();

        if (body)
        {
            body.constraints = RigidbodyConstraints.None;
        }

        yield return new WaitForSeconds(DeathDuration);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    

}

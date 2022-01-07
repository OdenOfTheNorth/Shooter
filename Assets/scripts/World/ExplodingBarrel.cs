using System;
using UnityEditor;
using UnityEngine;

public class ExplodingBarrel : Death
{
    public GameObject ExplationEffect;
    public float MaxExplotionForce = 100;
    public float MaxDamage = 10;
    public float Radius = 10;

    private void OnDrawGizmos()
    {
        Gizmos.color = Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, Radius);
    }

    public override void Call()
    {
        print("ExplodingBarrel");
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);

        Instantiate(ExplationEffect, transform.position, Quaternion.identity);
        
        foreach (Collider col in colliders)
        {
            float distance = Vector3.Distance(transform.position, col.transform.position);
            float percentigeForce = distance / Radius;
            float forceToApply = Mathf.Lerp(0, MaxExplotionForce, 1 - percentigeForce);
            
            print("percentige Force" + percentigeForce);
            
            Rigidbody rigidbody = col.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                Vector3 dir = (rigidbody.position - transform.position).normalized;
                rigidbody.AddForce(dir * forceToApply, ForceMode.Impulse);
                //Vector3 dir = (rigidbody.position - transform.position).normalized;
                //rigidbody.AddExplosionForce(MaxExplotionForce, transform.position, Radius);
                //rigidbody.AddForce(dir * MaxExplotionForce, ForceMode.Impulse);
            }
            
            PlayerMovement player = col.GetComponent<PlayerMovement>();
            if (player)
            {
                Vector3 dir = (rigidbody.position - transform.position).normalized;
                rigidbody.AddForce(dir * forceToApply, ForceMode.Impulse);
            }

            HealthComponent health = col.GetComponent<HealthComponent>();
            if (health)
            {
                float damageAmount = MaxDamage * (1 - percentigeForce);
                print(damageAmount);
                health.TakeDamage(damageAmount);
            }
        }
        
        Destroy(gameObject);
    }
}

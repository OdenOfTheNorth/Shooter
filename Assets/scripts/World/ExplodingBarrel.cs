using UnityEngine;

public class ExplodingBarrel : ActionToDo
{
    public GameObject AudioSource;
    public GameObject ExplationEffect;
    public Transform trans;
    public float MaxExplotionForce = 10;
    public float ExplotionForceMultiplier = 10;
    public float MaxDamage = 10;
    public float Radius = 10;
    public bool Debug = false;
    public LayerMask mask;

    private void Awake()
    {
        trans = transform;
    }

    private void OnDrawGizmos()
    {
        if (!Debug)
        {
            return;
        }
        
        Gizmos.color = Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, Radius);
    }

    public override void Call()
    {
        //print("ExplodingBarrel");

        Vector3 pos = trans.position;
        
        if (ExplationEffect)
        {
            Instantiate(ExplationEffect, pos, Quaternion.identity);    
        }

        if (AudioSource)
        {
            Instantiate(AudioSource, pos, Quaternion.identity);    
        }

        //Collider[] colliders = Physics.OverlapSphere(pos, Radius);
        //Ray ray = new Ray(pos, Vector3.zero);

        RaycastHit[] sphereHits = Physics.SphereCastAll(pos, Radius, Vector3.down, 0,mask);
        
        //print("Objects hit with expolotion" + sphereHits.Length);
        foreach (RaycastHit col in sphereHits)
        {
            //if (col.transform.gameObject == gameObject)
            //{
            //    continue;
            //}
            
            Vector3 colPos = col.transform.TransformPoint(col.point);//col.transform.position
            float distance = Vector3.Distance(pos , colPos);//(pos - colPos).magnitude;

            if (distance > Radius)
            {
                print("distance = " + distance + " Radius = " + Radius + " pos = " + pos + " colPos = " + colPos);
                //continue;
            }

            float percentigeForce = distance / Radius;

            if (percentigeForce > 1)
            {
                percentigeForce = 1;
            }
            
            float damageAmount = MaxDamage * (1 - percentigeForce);
            //\n
            string data = ("distance = " + distance + "Radius = " + Radius + "percentigeForce = " + (1 - percentigeForce) + "damageAmount = " + damageAmount + "pos = " + pos + "colPos = " + colPos);
            
            //print(data);
            float forceToApply = Mathf.Lerp(0, MaxExplotionForce * ExplotionForceMultiplier, 1 - percentigeForce);
            
            Rigidbody rigidbody = col.rigidbody;
            if (!rigidbody)
            {
                //rigidbody = col.transform.gameObject.GetComponentInParent<Rigidbody>();
            }
            
            if (rigidbody)
            {
                Vector3 dir = (colPos - pos).normalized;
                rigidbody.AddForce(dir * forceToApply, ForceMode.Impulse);
            }

            HealthComponent health = col.transform.gameObject.GetComponent<HealthComponent>();

            if (!health)
            {
                health = col.transform.gameObject.GetComponentInParent<HealthComponent>();
            }
            
            if (health)
            {
                //print(damageAmount);
                health.TakeDamage(damageAmount);
            }
        }
        
        Destroy(gameObject);
    }
}

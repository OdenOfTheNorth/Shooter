using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBomberAI : EnemyAI
{
    public GameObject projectile;
    public Transform projectileSpawnPos;
    public float projectileForce;
    
    public float attackRange = 10;
    public float TimeBetweenAttacks;
    public bool attacking;
    
    public float normalSpeed = 0;
    public float rollSpeed = 25;
    public float rollDuration = 3;

    public bool isRolling = false;
    public float RollColdown = 3f;
    public float lastRollTime = -3;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        normalSpeed = agent.speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void Update()
    {
        if (Target == null)
        {
            return;
        }

        if (isRolling)
        {
            return;
        }
        
        if ((transform.position - Target.position).sqrMagnitude < attackRange * attackRange)
        {
            Attack();
        }
        else
        {
            ChaseTarget();
        }
    }

    void ChaseTarget()
    {
        if (Target == null && isRolling)
        {
            return;
        }
        agent.destination = Target.position;
    }

    void Attack()
    {
        if (isRolling)
        {
            return;
        }
        
        Vector3 pos = transform.position;
        
        agent.destination = pos;

        Vector3 targetVec = Target.position;
        targetVec.y = pos.y;
        
        transform.LookAt(targetVec);
        if (!attacking)
        {
            GameObject obj = Instantiate(projectile, projectileSpawnPos.position, Quaternion.identity);

            Bomb bomb = obj.GetComponent<Bomb>();
            bomb.parent = gameObject;
            
            Rigidbody body = obj.GetComponent<Rigidbody>();
            Vector3 dir = Target.position - pos;
            
            body.AddForce(dir.normalized * projectileForce, ForceMode.Impulse);
            body.AddForce(Vector3.up * 3, ForceMode.Impulse);
            
            attacking = true;

            Invoke(nameof(ResetAttack), TimeBetweenAttacks);
        }
    }

    public void InvokeRoll()
    {
        
        //Debug.Break();
    }

    public void Roll()
    {
        //Debug.Break();
        
        if (isRolling)
        {
            return;
        }

        if (Time.time - lastRollTime > RollColdown)
        {
            lastRollTime = Time.time;
        
            isRolling = true;

            Vector3 sideRoll = transform.right * Random.Range(-2, 2);
        
            Rigidbody body = GetComponent<Rigidbody>();
            
            body.AddForce(sideRoll.normalized * 100, ForceMode.Impulse);
            
            Invoke("finishedRoll", rollDuration);
        }
    }

    public void finishedRoll()
    {
        isRolling = false;
        agent.speed = normalSpeed;
        agent.destination = Target.position;
    }

    void ResetAttack()
    {
        attacking = false;
    }
}

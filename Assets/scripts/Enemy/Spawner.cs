using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Object")] 
    public GameObject objectToSpawn;

    public Transform target;
    [Header("WaveData")]
    public int SpawnAmount = 1;
    public int Waves = 6;
    public int SpawnIncreaseRate = 3;
    public float SpawnDistance = 10;
    public float SpawnColdown = 10;
    private float currentTimer = 0;
    private int currentWaves = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnInstances();
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer -= Time.deltaTime;
        
        if (currentTimer <= 0)
        {
            SpawnInstances();
            currentTimer = SpawnColdown;
        }
    }

    void SpawnInstances()
    {
        if (currentWaves > Waves)
        {
            return;
        }
        
        int enemyCount = SpawnAmount + (currentWaves * SpawnIncreaseRate);
        
        currentWaves++;
        
        for (int i = 0; i < enemyCount; i++)
        {
            float x = Random.Range(-50, 50);
            float z = Random.Range(-50, 50);
            
            Vector3 pos = new Vector3(x, 0, z);

            float magnetude = (target.position - pos).magnitude;
            
            while (SpawnDistance > Mathf.Abs(magnetude))
            {
                x = Random.Range(-50, 50);
                z = Random.Range(-50, 50);
            
                pos = new Vector3(x, 0, z);

                magnetude = Mathf.Abs((target.position - pos).magnitude);
            }

            if (objectToSpawn)
            {
                 GameObject obj = Instantiate(objectToSpawn, pos, Quaternion.identity, transform);
                 EnemyAI enemy = obj.GetComponent<EnemyAI>();
                 enemy.Target = target;
            }
            else
            {
                print("No instance selected");
            }
            
        }
    }
}

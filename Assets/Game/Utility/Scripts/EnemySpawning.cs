using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    public int spawnPointAmount;

    [SerializeField] public SpawnRatio[] spawnRatios;

    public List<Transform> spawnPoints; //procedurally generated --> turn into Vector3s

    public float spawnInterval; //in seconds
    public float intervalVariation; //in seconds
    [Range(0,1)]public float spawnRate; //chance of spawning on call (btwn 0, 1)

    public float minRange; //minimum distance from player enemies can be spawned
    public float maxRange; //vice fucking versa

    private float timeToSpawn;

    private float totalRatio = 0;

    private ScoreManager scoreManager;
    private PlayerLifeCycleHandler player;

    [Header("Swarms")]

    public Swarm activeSwarm;

    public float swarmInterval;
    public float swarmIntervalVariation;

    public int swarmEnemies;
    public int swarmEnemiesVariation;

    public float swarmLength;
    public float swarmLengthVariation;

    public bool swarmActive;

    private float timeToSwarm;


    [System.Serializable]
    public struct SpawnRatio
    {
        public GameObject enemy;
        public float spawnRate;

        public SpawnRatio(GameObject g, float r)
        {
            enemy = g;
            spawnRate = r;
        }
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerLifeCycleHandler>();
        scoreManager = GetComponent<ScoreManager>();
        foreach(SpawnRatio ratio in spawnRatios)
        {
            totalRatio += ratio.spawnRate;
        }
        timeToSwarm = swarmInterval + swarmIntervalVariation * Random.Range(-1f, 1f);
    }

    private void Update()
    {
        timeToSpawn -= Time.deltaTime;
        if (timeToSpawn <= 0) Spawn();

        timeToSwarm -= Time.deltaTime;
        if (timeToSwarm <= 0)
        {
            if(!swarmActive) Swarm();
            else
            {
                timeToSwarm = swarmInterval + swarmIntervalVariation * Random.Range(-1f, 1f);
                //activeSwarm = null;
                swarmActive = false;
            }
        }
    }

    public void Spawn()
    {
        //                                                                  needs to be float                                               needs to be float
        timeToSpawn = swarmActive ? activeSwarm.duration/activeSwarm.enemies : (spawnInterval + intervalVariation * Random.Range(-1f, 1f));

        if (swarmActive || Random.Range(0f, 1f) <= spawnRate)
        {
            Vector3? point = GetRandomSpawnPoint(spawnPoints);
            if (point != null)
            {
                GameObject g = GetRandomEnemy(spawnRatios);
                if (g != null)
                {
                    Instantiate(g, point.Value, Quaternion.identity).GetComponent<Enemy>().Init(player.GetComponent<EnemyTarget>(), scoreManager);
                }
            }
        }
    }

    public void Swarm(Swarm? swarm = null)
    {
        if(!swarm.HasValue)
        {
            int enemies = swarmEnemies + Mathf.FloorToInt(swarmEnemiesVariation * Random.Range(-1f, 1f));
            float duration = swarmLength + swarmLengthVariation * Random.Range(-1f, 1f);
            activeSwarm = new Swarm(enemies, duration);
        }
        else activeSwarm = swarm.Value;

        timeToSwarm = activeSwarm.duration;

        swarmActive = true;

        //give player warning
        print("swarm approaching");
    }

    public GameObject GetRandomEnemy(SpawnRatio[] enemies)
    {
        float n = Random.Range(0, totalRatio);
        foreach(SpawnRatio ratio in enemies)
        {
            n -= ratio.spawnRate;
            if (n <= 0f) return ratio.enemy;
        }
        return null;
    }

    public Vector3? GetRandomSpawnPoint(List<Transform> spawnPoints)
    {
        List<Vector3> points = new List<Vector3>();
        foreach(Transform p in spawnPoints)
        {
            points.Add(p.position);
        }
        
        while(points.Count > 0)
        {
            Vector3 point = points[Random.Range(0, points.Count)];
            float d = Vector3.Distance(point, player.transform.position);
            if (d > minRange && d < maxRange) return point;
            points.Remove(point);
        }
        Debug.LogWarning("Spawning failed. No available spawn points in proper range of player.");
        return null;
    }
}

public struct Swarm
{
    public int enemies;
    public float duration;              //in seconds
    public float startRatio;      //% of enemies that spawn immediately on activation. 0 = completely over time

    public Swarm(int e, float t, float r = 0.1f)
    {
        enemies = e;
        duration = t;
        startRatio = Mathf.Clamp(r, 0, 1);
    }
}
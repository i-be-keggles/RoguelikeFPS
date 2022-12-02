using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    [SerializeField] public SpawnRatio[] spawnRatios;

    public Transform[] spawnPoints; //procedurally generated --> turn into Vector3s

    public float spawnInterval; //in seconds
    public float intervalVariation; //in seconds
    [Range(0,1)]public float spawnRate; //chance of spawning on call (btwn 0, 1)

    public float minRange; //minimum distance from player enemies can be spawned
    public float maxRange; //vice fucking versa

    private float timeToSpawn;

    private float totalRatio = 0;

    private PlayerLifeCycleHandler player;

    public float swarmInterval;
    public float swarmIntervalVariation;

    [Range(0,1)]public float swarmSpawnRate;
    public float swarmSpawnInterval;

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
                swarmActive = false;
            }
        }
    }

    public void Spawn()
    {
        //                                                                  needs to be float                                               needs to be float
        timeToSpawn = swarmActive ? (swarmInterval + swarmIntervalVariation * Random.Range(-1f, 1f)) : (spawnInterval + intervalVariation * Random.Range(-1f, 1f));

        for(int i = 0; i < spawnPoints.Length; i++)
        {
            float d = Vector3.Distance(spawnPoints[i].position, player.transform.position);
            if (d < maxRange & d > minRange)
            { 
                if(Random.Range(0f,1f) <= spawnRate)
                {
                    GameObject g = GetRandomEnemy(spawnRatios);
                    if(g != null) Instantiate(g, spawnPoints[i].position, Quaternion.identity);
                }
            }
        }
    }

    public void Swarm()
    {
        timeToSwarm = swarmLength + swarmLengthVariation * Random.Range(-1f, 1f);

        //give player warning
        print("swarm approaching");

        swarmActive = true;
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
}
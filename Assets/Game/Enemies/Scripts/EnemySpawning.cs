using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    public Dictionary<GameObject, float> spawnRatios;

    public Transform[] spawnPoints; //procedurally generated --> turn into Vector3s

    public float spawnInterval; //in seconds
    public float intervalVariation; //in seconds
    public float spawnRate; //chance of spawning on call (btwn 0, 1)

    public float minRange; //minimum distance from player enemies can be spawned
    public float maxRange; //vice fucking versa

    private float timeToSpawn;

    private float totalRatio = 0;

    private PlayerLifecycleHandler player;

    public float swarmInterval;
    public float swarmIntervalVariation;

    public float swarmSpawnRate;
    public float swarmSpawnInterval;

    public float swarmLength;
    public float swarmLengthVariation;

    public float swarmActive;

    private float timeToSwarm;

    private void Start()
    {
        player = FindObjectOfType(PlayerLifeCycleHandler);
        for(int i = 0; i < spawnRatios.Count; i++)
        {
            totalRatio += spawnRatios[i];
        }
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
        timeToSpawn = swarming? (swarmInterval + swarmIntervalVariation * Random.range(-1f, 1f)) : (spawnInterval + intervalVariation * Random.range(-1f, 1f));

        for(int i = 0; i < spawnPoints; i++)
        {
            float d = Vector3.Distance(spawnPoints[i].position, player.transform.position);
            if (d < maxRange & d > minRange)
            { 
                if(Random.range(0f,1f) <= spawnRate)
                {
                    int n = GetRandomEnemy(spawnRatios);
                    Instantiate(spawnRatios[n].Key);
                }
            }
        }
    }

    public void Swarm()
    {
        timeToSwarm += swarmInterval + swarmIntervalVariation * Random.range(-1f, 1f);

        //give player warning
        print("swarm approaching");

        swarming = true;
    }

    public int GetRandomEnemy(Dictionary enemies)
    {
        float i = 0;
        for(int i = Random.Range(0, spawnRatios); n > 0; i++)
        {
            n -= enemies[i];
        }

        return i;
    }
}
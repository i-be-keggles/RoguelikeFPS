using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MissionManager : MonoBehaviour
{
    public float time; //amount of time passed since starting

    public List<Objective> objectives;

    public EnemySpawning spawnManager;
    public ScoreManager scoreManager;


    private void Update()
    {
        time += Time.deltaTime;
    }

    public void CompleteObjective(int o)
    {
        Objective objective = objectives[o];
        float r = objective.Complete(time, spawnManager);
        scoreManager.AddScore(r);
        print($"Objective '{objective.name}' complete! +{r} points!");
    }

    [Serializable]
    public class Objective
    {
        public string name;
        public bool completed;
        protected float time; //amount of time taken to complete (if uncompleted, stores time it was initialised)

        public float rewards;

        public Swarm? swarm; //optional swarm triggered on completion

        protected MissionManager manager;

        public event EventHandler onCompleted;

        public Objective(string s, bool c, float r, float t, MissionManager m)
        {
            name = s;
            completed = c;
            rewards = r;
            time = t;
            manager = m;
        }

        public float Complete(float t, EnemySpawning spawnManager)
        {
            completed = true;
            time = t - time;

            if (swarm.HasValue) spawnManager.Swarm(swarm.Value);

            if(onCompleted != null)
            onCompleted.Invoke(this, EventArgs.Empty);

            return rewards;
        }
    }
}
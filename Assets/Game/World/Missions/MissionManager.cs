using UnityEngine;

public abstract class MissionManager : Monobehaviour
{
    public float time; //amount of time passed since starting

    public Objective[] objectives;

    public EnemySpawning spawnManager;


    private void Update()
    {
        time += Time.deltaTime;
    }

    public void CompleteObjective(int o)
    {
        Objective objective = objectives[o];
        float r = objective.Complete(time)
        print($"Objective '{objective.name}' complete! +{r} points!")
    }

    public struct Objective
    {
        public string name;
        public bool completed;
        protected float time; //amount of time taken to complete (if uncompleted, stores time it was initialised)

        public float rewards;

        public Swarm? swarm; //optional swarm triggered on completion


        public Objective(string s, bool c, float r, float t)
        {
            name = s;
            completed = c;
            rewards r;
            time = t;
        }

        public float Complete(t)
        {
            completed = true;
            time = t - time;

            if (swarm.HasValue) spawnManager.Swarm(swarm.Value);

            return rewards;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public List<LevelObject> objectPrefabs;
    public List<List<GameObject>> instances;
    public MapGenerator map;

    public LayerMask terrainMask;

    public List<GameObject> POIs;


    private void Start()
    {
        instances = new List<List<GameObject>>();
    }

    public void Generate()
    {
        POIs = new List<GameObject>();
        if(instances != null) for(int i = 0; i < instances.Count; i++) if (instances[i] != null) for (int j = 0; j < instances[i].Count; j++) if(instances[i][j] != null) DestroyImmediate(instances[i][j]);
        instances = new List<List<GameObject>>();

        float worldSize = map.mapSize * map.chunkSize;
        for (int i = 0; i < objectPrefabs.Count; i++)
        {
            instances.Add(new List<GameObject>());
            LevelObject obj = objectPrefabs[i];

            int goal = SizeVariation.RandomSizeInt(obj.instances, obj.instancesVariation);
            int spawned = 0;
            int attempts = 0;

            while(spawned < goal && attempts < goal * 10)
            {
                attempts++;
                Vector3 pos = transform.position + new Vector3(UnityEngine.Random.Range(-worldSize, worldSize) - map.chunkSize/2f, map.meshHeightMultiplier * 2, UnityEngine.Random.Range(-worldSize, worldSize) - map.chunkSize / 2f);
                RaycastHit hit;
                if(Physics.Raycast(pos, -Vector3.up, out hit, map.meshHeightMultiplier * 10, terrainMask))
                {
                    if(Vector3.Angle(Vector3.up, hit.normal) <= obj.angleCutoff && Physics.OverlapSphere(hit.point, obj.space, ~(terrainMask)).Length == 0)
                    {
                        GameObject go = Instantiate(obj.prefab, hit.point, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0f, 360f), 0)), hit.transform);
                        instances[i].Add(go);
                        spawned++;
                        if (obj.poi) POIs.Add(go);
                    }
                }
            }
            if(spawned < goal)
            {
                Debug.LogWarning("Could only find enough space for " + spawned + "/" + goal + " instances of " + obj.name);
            }
        }
    }

    [System.Serializable]
    public struct LevelObject
    {
        public string name;
        public int instances;
        [Range(0,1)] public float instancesVariation;
        public GameObject prefab;
        public float space;
        [Range(0,90)] public float angleCutoff;
        public bool poi;

        public LevelObject (string n, int i, float s, GameObject g, float r, float a, bool p)
        {
            name = n;
            instances = i;
            instancesVariation = s;
            prefab = g;
            space = r;
            angleCutoff = a;
            poi = p;
        }
    }
}
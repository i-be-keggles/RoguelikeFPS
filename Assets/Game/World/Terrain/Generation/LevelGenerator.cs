using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.Burst.CompilerServices;

public class LevelGenerator : MonoBehaviour
{    
    public Terrain terrain;
    public UnityTerrainGenerator gen;
    public EnemySpawning spawnManager;

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
        if(spawnManager.spawnPoints != null) foreach(Transform t in spawnManager.spawnPoints) DestroyImmediate(t.gameObject);
        instances = new List<List<GameObject>>();
        spawnManager.spawnPoints = new List<Transform>();

        float worldSize = terrain.terrainData.size.x;

        for (int x = 0; x < spawnManager.spawnPointAmount; x++)
        {
            for (int y = 0; y < spawnManager.spawnPointAmount; y++)
            {
                float ax = UnityEngine.Random.Range(x - 1, x + 1);
                float ay = UnityEngine.Random.Range(y - 1, y + 1);

                //in range of 0-1 (% of total terrain size)
                Vector3 pos = new Vector3(ax / spawnManager.spawnPointAmount, 0, ay / spawnManager.spawnPointAmount);

                if (pos.x < 0 || pos.x > 1 || pos.z < 0 || pos.z > 1) continue;

                float h = terrain.SampleHeight(pos * terrain.terrainData.size.x + transform.position);

                float angle = Vector3.Angle(terrain.terrainData.GetInterpolatedNormal(pos.x, pos.z), Vector3.up);
                if (angle <= 45f)
                {
                    //convert to worldspace
                    pos = pos * terrain.terrainData.size.x + transform.position;
                    pos.y = h + transform.position.y;
                    GameObject go = new GameObject("Spawn point " + (x* spawnManager.spawnPointAmount + y));
                    go.transform.parent = spawnManager.transform.GetChild(0);
                    go.transform.position = pos;
                    spawnManager.spawnPoints.Add(go.transform);
                }
            }
        }

        for (int i = 0; i < objectPrefabs.Count; i++)
        {
            instances.Add(new List<GameObject>());
            LevelObject obj = objectPrefabs[i];

            int goal = SizeVariation.RandomSizeInt(obj.instances, obj.instancesVariation);
            int spawned = 0;
            int attempts = 0;

            while(spawned < goal && attempts < goal * 30)
            {
                attempts++;
                float rx = UnityEngine.Random.Range(worldSize * (1 - obj.mapBorder), worldSize * obj.mapBorder);
                float ry = UnityEngine.Random.Range(worldSize * (1 - obj.mapBorder), worldSize * obj.mapBorder);
                Vector3 pos = transform.position + new Vector3(rx, terrain.terrainData.size.y * 2, ry);
                RaycastHit hit;
                if(Physics.Raycast(pos, -Vector3.up, out hit, terrain.terrainData.size.y * 3, terrainMask))
                {
                    float h = hit.point.y - transform.position.y;

                    bool validAngle = Vector3.Angle(Vector3.up, hit.normal) <= obj.angleCutoff;
                    if(obj.slopeVarArea > 0)
                    {
                        validAngle = gen.HeightVariationAtPoint(hit.point, obj.slopeVarArea) < obj.slopeVarCutoff;
                    }
                    if (h <= obj.maxHeight && h >= obj.minHeight && validAngle && Physics.OverlapSphere(hit.point, obj.space, ~(terrainMask)).Length == 0)
                    {
                        GameObject go = Instantiate(obj.prefab, hit.point, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0f, 360f), 0)), transform);
                        instances[i].Add(go);
                        spawned++;
                        
                        if (obj.poi) POIs.Add(go);
                        
                        CompoundGenerator g = go.GetComponent<CompoundGenerator>();
                        if (g != null) g.Init();
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
        public float slopeVarArea;
        public float slopeVarCutoff;
        public float maxHeight;
        public float minHeight;

        public bool poi;
        [Range(0, 1)] public float mapBorder;

        public LevelObject (string n, int i, float s, GameObject g, float r, float a, bool p, float mn, float mx = 100000f, float va = 2f, float vc = 0.5f, float mb = 1f)
        {
            name = n;
            instances = i;
            instancesVariation = s;
            prefab = g;
            space = r;
            angleCutoff = a;
            slopeVarArea = va;
            slopeVarCutoff = vc;
            maxHeight = mx;
            minHeight = mn;
            poi = p;
            mapBorder = mb;
        }
    }
}
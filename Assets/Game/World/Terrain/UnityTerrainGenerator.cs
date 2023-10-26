using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.TerrainUtils;
using Unity.AI.Navigation;

public class UnityTerrainGenerator : MonoBehaviour
{
    public Terrain terrain;

    [Min(0)] public int octaves;
    [Range(0, 1)] public float persistance;
    [Range(1, 3)] public float lacunarity;
    public float noiseScale;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public int seed;
    public Vector2 offset;

    public bool buildNavMeshOnPlay;
    private bool playing = false;

    [Space]
    public List<PlantFoliage> plants;
    public int plantDensity;

    [Space]
    public List<TreeFoliage> trees;
    public float treeDensity;
    private List<GameObject> spawnedTrees;
    public float treeDisplacement;
    public float treeNoiseScale;
    public float treeNoiseCutoff;

    [Space]
    public List<RockFoliage> rocks;
    public float rockDensity;
    private List<GameObject> spawnedRocks;
    public float rockDisplacement;

    private void Start()
    {
        playing = true;
        terrain = GetComponent<Terrain>();
        terrain.drawTreesAndFoliage = true;
        GenerateMap();
        TerrainCollider col = GetComponent<TerrainCollider>();
        col.enabled = false;
        col.enabled = true;
    }


    public void GenerateMap()
    {
        int size = terrain.terrainData.heightmapResolution;
        int x = 0; //use these later for chunks
        int y = 0;
        float[,] noiseMap = Noise.GenerateNoiseMap(size, size, seed, noiseScale, octaves, persistance, lacunarity, offset + new Vector2(x * (size - 1), y * (size - 1)), meshHeightCurve);
        terrain.terrainData.SetHeights(0, 0, noiseMap);

        //FlattenArea(100f, transform.position + new Vector3(500f, 80f, 500f), true);

        for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);

        GetComponent<LevelGenerator>().Generate();

        GeneratePlants();
        GenerateTrees();
        GenerateRocks();
        terrain.Flush();

        if(!playing || buildNavMeshOnPlay) GetComponent<NavMeshSurface>().BuildNavMesh();

    }

    public int[,] GenerateDetailMap(PlantFoliage plant)
    {
        int size = terrain.terrainData.detailResolution;
        int[,] detail = new int[size, size];
        for(int y = 0; y < size; y++)
            for(int x = 0; x < size; x++)
            {
                float angle = Vector3.Angle(terrain.terrainData.GetInterpolatedNormal(y / (float)size, x / (float)size), Vector3.up);

                detail[x, y] = ((UnityEngine.Random.Range(0f, 1f) <= plant.density) && (angle <= plant.slopeCutoff)) ? plantDensity : 0;
            }
                
        return detail;
    }

    public void GeneratePlants()
    {
        DetailPrototype[] detailPrototypes = new DetailPrototype[plants.Count];
        for (int i = 0; i < plants.Count; i++)
        {
            DetailPrototype p = new DetailPrototype();
            p.prototype = plants[i].mesh;
            p.useInstancing = true;
            p.usePrototypeMesh = true;
            p.renderMode = DetailRenderMode.VertexLit;
            detailPrototypes[i] = p;
        }

        terrain.terrainData.detailPrototypes = detailPrototypes;

        for (int i = 0; i < plants.Count; i++)
        {
            terrain.terrainData.SetDetailLayer(0, 0, i, GenerateDetailMap(plants[i]));
        }
    }

    public void GenerateTrees()
    {
        //if(spawnedTrees != null)foreach (GameObject tree in spawnedTrees) if(tree != null) DestroyImmediate(tree);
        spawnedTrees = new List<GameObject>();

        TreePrototype[] treePrototypes = new TreePrototype[trees.Count];
        for (int i = 0; i < trees.Count; i++)
        {
            TreePrototype p = new TreePrototype();
            p.prefab = trees[i].GetPrefab();
            treePrototypes[i] = p;
        }
        terrain.terrainData.treePrototypes = treePrototypes;

        float treesPerAxis = treeDensity * terrain.terrainData.size.x / 10f;

        float[,] noiseMap = Noise.GenerateNoiseMap((int)treesPerAxis, (int)treesPerAxis, seed, treeNoiseScale, 3, 0.2f, 1.5f, offset);

        for (int i = 0; i < trees.Count; i++)
        {
            for (int x = 0; x < treesPerAxis; x++)
            {
                for (int y = 0; y < treesPerAxis; y++)
                {
                    float ax = UnityEngine.Random.Range(x - treeDisplacement, x + treeDisplacement);
                    float ay = UnityEngine.Random.Range(y - treeDisplacement, y + treeDisplacement);

                    //in range of 0-1 (% of total terrain size)
                    Vector3 pos = new Vector3(ax / treesPerAxis, 0, ay / treesPerAxis);

                    if (pos.x < 0 || pos.x > 1 || pos.z < 0 || pos.z > 1) continue;

                    float h = terrain.SampleHeight(pos * terrain.terrainData.size.x + transform.position);

                    float angle = Vector3.Angle(terrain.terrainData.GetInterpolatedNormal(pos.x, pos.z), Vector3.up);
                    if (h / terrain.terrainData.size.y <= trees[i].maxHeight && treeNoiseCutoff <= noiseMap[(int)(pos.x*treesPerAxis), (int)(pos.z * treesPerAxis)] && angle <= trees[i].slopeCutoff)
                    {
                        //convert to worldspace
                        pos = pos * terrain.terrainData.size.x + transform.position;
                        pos.y = h + transform.position.y;
                        GameObject go = Instantiate(trees[i].mesh, pos, Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0), transform);
                        spawnedTrees.Add(go);
                    }
                }
            }
        }
    }

    public void GenerateRocks()
    {
        //if (spawnedRocks != null) foreach (GameObject rock in spawnedRocks) if (rock != null) DestroyImmediate(rock);
        spawnedRocks = new List<GameObject>();

        TreePrototype[] treePrototypes = terrain.terrainData.treePrototypes;
        for (int i = 0; i < rocks.Count; i++)
        {
            for(int j = 0; j < rocks[i].mesh.Length; j++)
            {
                TreePrototype p = new TreePrototype();
                p.prefab = rocks[i].mesh[j];
                treePrototypes.Append(p);
            }
        }
        terrain.terrainData.treePrototypes = treePrototypes;

        float rocksPerAxis = rockDensity * terrain.terrainData.size.x / 10f;

        for (int i = 0; i < rocks.Count; i++)
        {
            for (int x = 0; x < rocksPerAxis; x++)
            {
                for (int y = 0; y < rocksPerAxis; y++)
                {
                    float ax = UnityEngine.Random.Range(x - rockDisplacement, x + rockDisplacement);
                    float ay = UnityEngine.Random.Range(y - rockDisplacement, y + rockDisplacement);

                    //in range of 0-1 (% of total terrain size)
                    Vector3 pos = new Vector3(ax / rocksPerAxis, 0, ay / rocksPerAxis);

                    if (pos.x < 0 || pos.x > 1 || pos.z < 0 || pos.z > 1) continue;

                    float h = terrain.SampleHeight(pos * terrain.terrainData.size.x + transform.position);

                    float angle = Vector3.Angle(terrain.terrainData.GetInterpolatedNormal(pos.x, pos.z), Vector3.up);
                    if (h / terrain.terrainData.size.y <= rocks[i].maxHeight && angle <= rocks[i].slopeCutoff)
                    {
                        //convert to worldspace
                        pos = pos * terrain.terrainData.size.x + transform.position;
                        pos.y = h + transform.position.y;
                        GameObject go = Instantiate(rocks[i].GetPrefab(), pos, Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0), transform);
                        spawnedRocks.Add(go);
                    }
                }
            }
        }
    }

    public Vector3 PosFromDetailIndex(int x, int y)
    {
        float d = terrain.terrainData.detailResolution;
        float s = terrain.terrainData.size.x;
        Vector3 pos = transform.position + new Vector3(s * (x / d), 0, s * (y / d));
        pos.y = terrain.SampleHeight(pos);
        return pos;
    }

    public void FlattenArea(float radius, Vector3 pos, bool local = false)
    {
        if(!local) pos -= transform.position;
        float pixelRadius = radius * terrain.terrainData.heightmapResolution / terrain.terrainData.size.x;
        int pixelSize = Mathf.RoundToInt(pixelRadius * 1.3f);
        Vector2 pixelPoint = new Vector2(pos.x, pos.z) * terrain.terrainData.heightmapResolution / terrain.terrainData.size.x;
        int sx = Math.Clamp(Mathf.RoundToInt(pixelPoint.x - pixelSize), 0, terrain.terrainData.heightmapResolution-1);
        int sy = Math.Clamp(Mathf.RoundToInt(pixelPoint.y - pixelSize), 0, terrain.terrainData.heightmapResolution-1);
        float[,] heights = HeightmapUtil.NormaliseToHeight(terrain.terrainData.GetHeights(sx, sy, pixelSize * 2, pixelSize * 2), pixelPoint, pos.y / terrain.terrainData.size.y, pixelRadius);
        terrain.terrainData.SetHeights(sx, sy, heights);

        terrain.Flush();
    }
}

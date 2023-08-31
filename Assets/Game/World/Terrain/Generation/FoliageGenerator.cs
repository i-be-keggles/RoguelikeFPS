using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using System;

public class FoliageGenerator : MonoBehaviour
{
    public LayerMask terrainMask;

    public PlantFoliage[] plants;
    public float plantDensity;
    public float displacement;

    [Space]
    public TreeFoliage[] trees;
    public float treeDensity;
    public float treeNoiseScale;
    public float[,] treeNoiseMap;
    public Texture2D treeNoiseMapTex;
    private List<GameObject> spawnedTrees;

    [Space]
    public RockFoliage[] rocks;
    public float rockDensity;

    [Space]
    public List<List<Vector3>> grassPositions = new List<List<Vector3>>();

    public List<List<List<Matrix4x4>>> batches = new List<List<List<Matrix4x4>>>();

    private bool playing = false;

    [Space]
    public Texture2D grassDisplacementMap;
    public Texture2D foliageDensityMap;
    public int foliageDensityMapResolution = 512;

    public MapGenerator map;


    private void Awake()
    {
        playing = true;
        spawnedTrees = new List<GameObject>();
    }

    private void Update()
    {
        RenderGrassBatches();
    }

    private void DrawToDensity(Foliage foliage, Vector3 position)
    {
        float radius = foliage.densityRadius / map.chunkSize * foliageDensityMapResolution;
        Vector2 pos = new Vector2(position.x - (transform.position.x - map.chunkSize / 2), position.z - (transform.position.z - map.chunkSize / 2)) / map.chunkSize * foliageDensityMapResolution;
        for (int x = Mathf.Max(Mathf.FloorToInt(pos.x - radius) - 1, 0); x < Mathf.Min(Mathf.FloorToInt(pos.x + radius) + 1, foliageDensityMapResolution); x++)
            for (int y = Mathf.Max(Mathf.FloorToInt(pos.y - radius) - 1, 0); y < Mathf.Min(Mathf.FloorToInt(pos.y + radius) + 1, foliageDensityMapResolution); y++)
            {
                float dx = x - pos.x;
                float dy = y - pos.y;
                float d = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
                float m = 1 - Explosion.GetFalloff(radius - d, radius);
                Color c = foliageDensityMap.GetPixel(x, y);
                int id = foliage.GetID();
                foliageDensityMap.SetPixel(x, y, c + new Color(id == 0? m : 0, id == 1 ? m : 0, id == 2 ? m : 0));
            }
        foliageDensityMap.Apply();
    }

    private Color GetDensityAtPosition(Vector3 position)
    {
        Vector2 pos = new Vector2(position.x - (transform.position.x - map.chunkSize / 2), position.z - (transform.position.z - map.chunkSize / 2)) / map.chunkSize * foliageDensityMapResolution;
        return foliageDensityMap.GetPixel((int)pos.x, (int)pos.y);
    }

    public void GenerateGrass(Transform chunk, float heightMultiplier, AnimationCurve heightCurve)
    {
        if (!playing) return;

        foliageDensityMap = new Texture2D(foliageDensityMapResolution, foliageDensityMapResolution, TextureFormat.ARGB32, false);
        for (int x = 0; x < foliageDensityMapResolution; x++)
            for (int y = 0; y < foliageDensityMapResolution; y++)
            {
                foliageDensityMap.SetPixel(x, y, new Color(0, 0, 0));
            }

        foreach (PlantFoliage plant in plants)
        {
            grassPositions.Add(new List<Vector3>());
            batches.Add(new List<List<Matrix4x4>>());
        }
        
        for(int i = 0; i < plants.Length; i++)
        {
            if (plants[i].clusterSize > 0)
            {
                for (int j = 0; j < plants[i].density * map.chunkSize; j++)
                {
                    Vector3 position = chunk.position + new Vector3(UnityEngine.Random.Range(-map.chunkSize, map.chunkSize)/2f, heightMultiplier * 2, UnityEngine.Random.Range(-map.chunkSize, map.chunkSize)/2f);
                    GenerateFoliageCluster(plants[i], position, i, chunk);
                }
            }
            else for (int x = 0; x < plantDensity; x++)
                for(int y = 0; y < plantDensity; y++)
                {
                    float ax = UnityEngine.Random.Range(x - displacement, x + displacement);
                    float ay = UnityEngine.Random.Range(y - displacement, y + displacement);

                    Vector3 offset = new Vector3(-map.chunkSize / 2f, 0, map.chunkSize / 2f);
                    RaycastHit hit;
                    if(Physics.Raycast(chunk.position + offset + new Vector3(ax * map.chunkSize / plantDensity, heightMultiplier * 2, -ay * map.chunkSize / plantDensity), -Vector3.up, out hit, heightMultiplier*5, terrainMask)){
                        if(hit.transform == chunk) TryPlaceFoliage(plants[i], hit.point, hit.normal, i, chunk);
                    }
                }
        }
    }

    public void GenerateTrees(Transform chunk, float heightMultiplier, AnimationCurve heightCurve)
    {
        if (!playing) return;

        foreach (GameObject tree in spawnedTrees) Destroy(tree);
        spawnedTrees = new List<GameObject>();

        int noiseSize = (int) treeDensity * map.chunkSize / 10;
        treeNoiseMap = Noise.GenerateNoiseMap(noiseSize, noiseSize, map.seed, treeNoiseScale, 2, 0.2f, 1.5f, new Vector2(0.234234f, 0.12412f));
        //treeNoiseMap = new float[noiseSize, noiseSize];

        treeNoiseMapTex = new Texture2D(noiseSize, noiseSize, TextureFormat.ARGB32, false);
        for (int x = 0; x < noiseSize; x++)
            for (int y = 0; y < noiseSize; y++)
            {
                //treeNoiseMap[x, y] = Mathf.PerlinNoise(x * treeNoiseScale, y * treeNoiseScale);
                float r = treeNoiseMap[x, y];
                treeNoiseMapTex.SetPixel(x, y, new Color(r,r,r));
            }
        treeNoiseMapTex.Apply();

        for (int i = 0; i < trees.Length; i++)
        {
            if (trees[i].clusterSize > 0)
            {
                for (int j = 0; j < trees[i].density * map.chunkSize; j++)
                {
                    Vector3 position = chunk.position + new Vector3(UnityEngine.Random.Range(-map.chunkSize, map.chunkSize) / 2f, heightMultiplier * 2, UnityEngine.Random.Range(-map.chunkSize, map.chunkSize) / 2f);
                    GenerateFoliageCluster(trees[i], position, i, chunk);
                }
            }
            else for (int x = 0; x < treeDensity; x++)
                    for (int y = 0; y < treeDensity; y++)
                    {
                        float ax = UnityEngine.Random.Range(x - displacement, x + displacement);
                        float ay = UnityEngine.Random.Range(y - displacement, y + displacement);

                        Vector3 offset = new Vector3(-map.chunkSize / 2f, 0, map.chunkSize / 2f);
                        RaycastHit hit;
                        if (Physics.Raycast(chunk.position + offset + new Vector3(ax * map.chunkSize / treeDensity, heightMultiplier * 2, -ay * map.chunkSize / treeDensity), -Vector3.up, out hit, heightMultiplier * 5, terrainMask))
                        {
                            if (hit.transform == chunk) TryPlaceFoliage(trees[i], hit.point, hit.normal, i, chunk);
                        }
                    }
        }
    }

    public void GenerateRocks(Transform chunk, float heightMultiplier, AnimationCurve heightCurve)
    {
        if (!playing) return;

        for (int i = 0; i < rocks.Length; i++)
        {
            if (rocks[i].clusterSize > 0)
            {
                for (int j = 0; j < rocks[i].density * map.chunkSize; j++)
                {
                    Vector3 position = chunk.position + new Vector3(UnityEngine.Random.Range(-map.chunkSize, map.chunkSize) / 2f, heightMultiplier * 2, UnityEngine.Random.Range(-map.chunkSize, map.chunkSize) / 2f);
                    GenerateFoliageCluster(rocks[i], position, i, chunk);
                }
            }
            else for (int x = 0; x < rockDensity; x++)
                    for (int y = 0; y < rockDensity; y++)
                    {
                        float ax = UnityEngine.Random.Range(x - displacement, x + displacement);
                        float ay = UnityEngine.Random.Range(y - displacement, y + displacement);

                        Vector3 offset = new Vector3(-map.chunkSize / 2f, 0, map.chunkSize / 2f);
                        RaycastHit hit;
                        if (Physics.Raycast(chunk.position + offset + new Vector3(ax * map.chunkSize / rockDensity, heightMultiplier * 2, -ay * map.chunkSize / rockDensity), -Vector3.up, out hit, heightMultiplier * 5, terrainMask))
                        {
                            if (hit.transform == chunk) TryPlaceFoliage(rocks[i], hit.point, hit.normal, i, chunk);
                        }
                    }
        }
    }

    public void GenerateFoliageCluster(Foliage foliage, Vector3 position, int plantIndex, Transform chunk)
    {
        int n = Mathf.RoundToInt(SizeVariation.RandomSize(foliage.clusterSize * foliage.clusterDensity, foliage.clusterVariance));
        for (int i = 0; i < n; i++)
        {
            RaycastHit hit;
            Vector2 pos = UnityEngine.Random.insideUnitCircle * foliage.clusterSize;
            if (Physics.Raycast(new Vector3(position.x + pos.x, position.y, position.z + pos.y), -Vector3.up, out hit, 200, terrainMask))
            {
                if (hit.transform == chunk) TryPlaceFoliage(foliage, hit.point, hit.normal, plantIndex, chunk, foliage.GetID() != 0);
            }
        }
    }

    public bool TryPlaceFoliage(Foliage foliage, Vector3 position, Vector3 normal, int plantIndex, Transform chunk, bool selfDensity=true, bool othersDensity=true)
    {
        float angle = Vector3.Angle(normal, Vector3.up);
        float spawnChance = foliage.SlopeProbability(angle) * (selfDensity ? foliage.density : 1) * (othersDensity ? (1 - Math.Clamp(GetDensityAtPosition(position).r, 0, 1)) : 1);

        if (foliage.GetID() == 1)
        {
            //Vector2 treeNoisePos = new Vector2(position.x - (transform.position.x - map.chunkSize / 2), position.z - (transform.position.z - map.chunkSize / 2)) / map.chunkSize * (treeDensity * map.chunkSize / 10);
            Vector2 treeNoisePos = new Vector2(position.x - (chunk.position.x - map.chunkSize / 2), position.z - (chunk.position.z - map.chunkSize / 2)) / map.chunkSize * (treeDensity * map.chunkSize / 10);
            spawnChance = foliage.SlopeProbability(angle) * treeNoiseMap[(int)treeNoisePos.x, (int)treeNoisePos.y];
        }
        
        if ((foliage.GetID() != 1? UnityEngine.Random.Range(0f, 1f) : 1 - foliage.density) < spawnChance)
        {
            if (foliage.GetID() == 0) grassPositions[plantIndex].Add(position);
            else
            {
                GameObject go = Instantiate(foliage.GetPrefab(), position + foliage.baseOffset, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0f, 360f), 0) + foliage.baseRotation), chunk);
                go.transform.localScale = go.transform.localScale * SizeVariation.RandomSize(1, 0.1f);
                spawnedTrees.Add(go);
            }
            DrawToDensity(foliage, position);
            return true;
        }
        else return false;
    }

    public void GenerateMatrices()
    {
        if (!playing) return;

        for (int i = 0; i < grassPositions.Count; i++)
        {
            int addedMatrices = 0;
            batches[i].Add(new List<Matrix4x4>());
            for (int j = 0; j < grassPositions[i].Count; j++)
            {
                if (addedMatrices < 1000)
                {
                    //print(i + " / " + batches.Count + ",  " + (batches[i].Count - 1) + " / " + batches[i].Count);
                    batches[i][batches[i].Count - 1].Add(Matrix4x4.TRS(grassPositions[i][j] + plants[i].baseOffset, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0f, 360f), 0) + plants[i].baseRotation), Vector3.one));
                    addedMatrices++;
                }
                else
                {
                    batches[i].Add(new List<Matrix4x4>());
                    addedMatrices = 0;
                }
            }
        }
    }

    private void RenderGrassBatches()
    {
        for(int i = 0; i < plants.Length; i++)
        foreach(var batch in batches[i])
        {
            for(int j = 0; j < plants[0].mesh.subMeshCount; j++)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetVector("_ParentPos", transform.position);
                block.SetFloat("_ParentSize", map.chunkSize);
                block.SetTexture("_DisplacementMap", grassDisplacementMap);
                Graphics.DrawMeshInstanced(plants[i].mesh, j, plants[i].material, batch, block, UnityEngine.Rendering.ShadowCastingMode.Off, false);
            }
        }
    }
}
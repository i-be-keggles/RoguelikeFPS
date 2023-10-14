using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

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


    public List<PlantFoliage> plants;
    public int plantDensity;

    public List<TreeFoliage> trees;
    public int treeDensity;

    private void Start()
    {
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

        GeneratePlants();
        GenerateTrees();
        terrain.Flush();
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
        TreePrototype[] treePrototypes = new TreePrototype[trees.Count];
        for (int i = 0; i < trees.Count; i++)
        {
            TreePrototype p = new TreePrototype();
            p.prefab = trees[i].GetPrefab();
            treePrototypes[i] = p;
        }
        terrain.terrainData.treePrototypes = treePrototypes;

        int size = terrain.terrainData.heightmapResolution;

        float[,] noiseMap = Noise.GenerateNoiseMap(size, size, seed, noiseScale, octaves, persistance, lacunarity, offset, meshHeightCurve);

        TreeInstance[] instances = new TreeInstance[0];
        terrain.terrainData.SetTreeInstances(instances, false);
        for (int i = 0; i < trees.Count; i++)
        {
            for (float y = 0; y < 50; y++)
                for (float x = 0; x < 50; x++)
                {
                    if (true || UnityEngine.Random.Range(0f, 1f) <= trees[i].density * treeDensity)
                    {
                        TreeInstance tree = new TreeInstance();

                        //Vector3 pos = transform.position + new Vector3(s * (x / size), 0, s * (y / size));
                        Vector3 pos = new Vector3(x / (float)size, 0, y / (float)size);
                        print(pos);
                        tree.position = pos;
                        tree.heightScale = UnityEngine.Random.Range(0.9f, 1.1f);
                        tree.widthScale = 1f;
                        tree.color = UnityEngine.Color.white;
                        tree.lightmapColor = UnityEngine.Color.white;
                        tree.prototypeIndex = 0;
                        //terrain.AddTreeInstance(tree);
                        instances.Append(tree);
                        //terrain.Flush();
                    }
                }
        }
        terrain.terrainData.SetTreeInstances(instances, true);
    }

    public Vector3 PosFromDetailIndex(int x, int y)
    {
        float d = terrain.terrainData.detailResolution;
        float s = terrain.terrainData.size.x;
        Vector3 pos = transform.position + new Vector3(s * (x / d), 0, s * (y / d));
        pos.y = terrain.SampleHeight(pos);
        return pos;
    }
}

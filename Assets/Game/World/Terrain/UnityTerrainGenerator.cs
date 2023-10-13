using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        terrain = GetComponent<Terrain>();
        GenerateMap();
    }


    public void GenerateMap()
    {
        int size = terrain.terrainData.heightmapResolution;
        int x = 0; //use these later for chunks
        int y = 0;
        float[,] noiseMap = Noise.GenerateNoiseMap(size, size, seed, noiseScale, octaves, persistance, lacunarity, offset + new Vector2(x * (size - 1), y * (size - 1)), meshHeightCurve);
        terrain.terrainData.SetHeights(0, 0, noiseMap);

        GenerateGrass();
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

    public void GenerateGrass()
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

    public Vector3 PosFromDetailIndex(int x, int y)
    {
        float d = terrain.terrainData.detailResolution;
        float s = terrain.terrainData.size.x;
        Vector3 pos = transform.position + new Vector3(s * (x / d), 0, s * (y / d));
        pos.y = terrain.SampleHeight(pos);
        return pos;
    }
}

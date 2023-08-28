using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using System;

public class FoliageGenerator : MonoBehaviour
{
    public PlantFoliage[] plants;
    public float density;
    public float displacement;

    public List<List<Vector3>> grassPositions = new List<List<Vector3>>();

    public List<List<List<Matrix4x4>>> batches = new List<List<List<Matrix4x4>>>();

    private bool playing = false;

    public Texture2D grassDisplacementMap;
    public Texture2D foliageDensityMap;
    public int foliageDensityMapResolution = 512;

    public MapGenerator map;



    private void Awake()
    {
        playing = true;
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

    public void GenerateGrass(float[,] heightMap, Transform chunk, float heightMultiplier, AnimationCurve heightCurve)
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
                    GenerateFoliageCluster(plants[i], position, i);
                }
            }
            else for (int x = 0; x < density; x++)
                for(int y = 0; y < density; y++)
                {
                    float ax = UnityEngine.Random.Range(x - displacement, x + displacement);
                    float ay = UnityEngine.Random.Range(y - displacement, y + displacement);

                    Vector3 offset = new Vector3(-heightMap.GetLength(0) / 2f, 0, heightMap.GetLength(1) / 2f);
                    RaycastHit hit;
                    if(Physics.Raycast(chunk.position + offset + new Vector3(ax * heightMap.GetLength(0) / density, heightMultiplier * 2, -ay * heightMap.GetLength(1) / density), -Vector3.up, out hit)){
                        TryPlacePlant(plants[i], hit.point, hit.normal, i);
                    }
                }
        }

    }

    public void GenerateFoliageCluster(PlantFoliage plant, Vector3 position, int plantIndex)
    {
        int n = Mathf.RoundToInt(SizeVariation.RandomSize(plant.clusterSize * plant.clusterDensity, plant.clusterVariance));
        for (int i = 0; i < n; i++)
        {
            RaycastHit hit;
            Vector2 pos = UnityEngine.Random.insideUnitCircle * plant.clusterSize;
            if (Physics.Raycast(new Vector3(position.x + pos.x, position.y, position.z + pos.y), -Vector3.up, out hit))
            {
                TryPlacePlant(plants[plantIndex], hit.point, hit.normal, plantIndex, false);
            }
        }
    }

    public bool TryPlacePlant(PlantFoliage plant, Vector3 position, Vector3 normal, int plantIndex, bool selfDensity=true, bool othersDensity=true)
    {
        float angle = Vector3.Angle(normal, Vector3.up);
        if (UnityEngine.Random.Range(0f, 1f) < plant.SlopeProbability(angle) * (selfDensity? plant.density : 1) * (othersDensity?(1 - Math.Clamp(GetDensityAtPosition(position).r, 0, 1)) : 1))
        {
            grassPositions[plantIndex].Add(position);
            DrawToDensity(plant, position);
            return true;
        }
        else return false;
    }

    public void GenerateMatrices()
    {
        if (!playing) return;

        print(grassPositions[0].Count + "   " + grassPositions[1].Count);
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
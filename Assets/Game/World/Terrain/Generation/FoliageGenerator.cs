using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using System;

public class FoliageGenerator : MonoBehaviour
{
    public Plant[] plants;
    public float density;
    public float angleCutoff;
    public float displacement;

    public List<List<Vector3>> grassPositions = new List<List<Vector3>>();

    public List<List<List<Matrix4x4>>> batches = new List<List<List<Matrix4x4>>>();

    private bool playing = false;

    public Texture2D grassDisplacementMap;

    public MapGenerator map;


    private void Awake()
    {
        playing = true;
    }

    private void Update()
    {
        RenderGrassBatches();
    }

    public void GenerateGrass(float[,] heightMap, Transform chunk, float heightMultiplier, AnimationCurve heightCurve)
    {
        if (!playing) return;

        foreach (Plant plant in plants)
        {
            grassPositions.Add(new List<Vector3>());
            batches.Add(new List<List<Matrix4x4>>());
        }
        
        for (int x = 0; x < density; x++)
            for(int y = 0; y < density; y++)
            {
                float ax = UnityEngine.Random.Range(x - displacement, x + displacement);
                float ay = UnityEngine.Random.Range(y - displacement, y + displacement);

                Vector3 offset = new Vector3(-heightMap.GetLength(0) / 2f, 0, heightMap.GetLength(1) / 2f);
                RaycastHit hit;
                if(Physics.Raycast(chunk.position + offset + new Vector3(ax * heightMap.GetLength(0) / density, heightMultiplier * 2, -ay * heightMap.GetLength(1) / density), -Vector3.up, out hit)){
                    if (Vector3.Dot(hit.normal, Vector3.up) > angleCutoff)
                    {
                        int grassType = UnityEngine.Random.Range(0, plants.Length);
                        grassPositions[grassType].Add(hit.point);
                    }
                }
            }
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

[Serializable]
public struct Plant
{
    public String name;
    public Mesh mesh;
    public Material material;
    public Vector3 baseRotation;
    public Vector3 baseOffset;

    public Plant(String s, Mesh m, Vector3 r, Vector3 d, Material t = null)
    {
        name = s;
        mesh = m;
        baseRotation = r;
        baseOffset = d;
        material = t;
    }
}
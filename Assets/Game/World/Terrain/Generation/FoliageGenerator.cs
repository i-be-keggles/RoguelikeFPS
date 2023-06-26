using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using System;

public class FoliageGenerator : MonoBehaviour
{
    public Mesh grass;
    public Material grassMaterial;
    public float density;
    public float angleCutoff;
    public float displacement;

    public List<Vector3> grassPositions = new List<Vector3>();

    private List<List<Matrix4x4>> batches = new List<List<Matrix4x4>>();

    private bool playing = false;

    public Texture2D grassDisplacement;

    public MapGenerator map;
    public Vector2 displacementOffset;
    public Color[] displacementMap;


    private void Awake()
    {
        playing = true;
    }

    private void Update()
    {
        RenderGrassBatches();
        displacementMap = grassDisplacement.GetPixels();
    }

    public void GenerateGrass(float[,] heightMap, Transform chunk, float heightMultiplier, AnimationCurve heightCurve)
    {
        if (!playing) return;
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
                        grassPositions.Add(hit.point);
                    }
                }
            }
    }

    public void GenerateMatrices()
    {
        if (!playing) return;

        int addedMatrices = 0;
        batches.Add(new List<Matrix4x4>());

        for (int i = 0; i < grassPositions.Count; i++)
        {
            if (addedMatrices < 1000)
            {
                batches[batches.Count - 1].Add(Matrix4x4.TRS(grassPositions[i], Quaternion.Euler(new Vector3(-90, UnityEngine.Random.Range(0f, 360f), 0)), Vector3.one));
                addedMatrices++;
            }
            else
            {
                batches.Add(new List<Matrix4x4>());
                addedMatrices = 0;
            }
        }
    }

    private void RenderGrassBatches()
    {
        foreach(var batch in batches)
        {
            for(int i = 0; i < grass.subMeshCount; i++)
            {
                Vector3 pos = batch[i].GetPosition();
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetColor("_Displacement", SampleDisplacement(new Vector2(pos.x - map.transform.position.x + map.chunkSize/2, pos.z - map.transform.position.z + map.chunkSize / 2) + displacementOffset, new Vector2(map.chunkSize, map.chunkSize)));
                Graphics.DrawMeshInstanced(grass, i, grassMaterial, batch, block, UnityEngine.Rendering.ShadowCastingMode.Off, false);
            }
        }
    }

    public Color SampleDisplacement(Vector2 pos, Vector2 size)
    {
        try
        {
            int s = grassDisplacement.height;
            float x = (pos.x / size.x) * s;
            float y = (pos.y / size.y) * s;
            return displacementMap[(int)(y*s + x)];
        }
        catch(Exception e)
        {
            //print(e.Message);
            return Color.white;
        }
    }
}

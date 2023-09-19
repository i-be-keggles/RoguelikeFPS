using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System;
using System.Linq;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode {NoiseMap, ColourMap, Mesh};
	public DrawMode drawMode;

    [Min(10)] public int chunkSize;
    [Min(1)] public int mapSize;
    public float noiseScale;

    [Min(0)] public int octaves;
	[Range(0,1)] public float persistance;
    [Range(1,3)] public float lacunarity;

	public int seed;
	public Vector2 offset;

	public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public MeshData[] chunks;

    public bool autoUpdate;
    public FoliageGenerator foliage;
    public LevelGenerator level;


    private void Start()
    {
        GenerateMap();
    }

    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    public void GenerateMap() {
        MapDisplay display = FindObjectOfType<MapDisplay>();

        foliage.grassPositions = new List<List<Vector3>>();
        foliage.batches = new List<List<List<Matrix4x4>>>();

        for(int x = 0; x < mapSize; x++)
            for(int y = 0; y < mapSize; y++)
            {
                float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset + new Vector2(x * (chunkSize -1), y * (chunkSize -1)));

                if (drawMode == DrawMode.Mesh)
                    display.DrawMesh(GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), new Vector3(x * (chunkSize -1), 0, y * (chunkSize -1)), new int2(x * mapSize + y, mapSize * mapSize), noiseMap);
                else
                    display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap), x * mapSize + y);
            }
        level.Generate();
        foreach(MapDisplay.TerrainChunk chunk in display.chunks)
        {
            Vector2 chunkPos = new Vector2(chunk.gameObject.transform.GetSiblingIndex() % mapSize, chunk.gameObject.transform.GetSiblingIndex() / mapSize);
            FoliageGenerator f = CopyComponent(foliage, chunk.gameObject);
            f.GenerateGrass(chunk.gameObject.transform, meshHeightMultiplier, meshHeightCurve);
            f.GenerateTrees(chunk.gameObject.transform, meshHeightMultiplier, meshHeightCurve, chunkPos);
            f.GenerateRocks(chunk.gameObject.transform, meshHeightMultiplier, meshHeightCurve);
        }
        foliage.GenerateMatrices();

    }

    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightMap[x, y] * heightMultiplier * heightCurve.Evaluate(heightMap[x,y]), topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;

    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

}
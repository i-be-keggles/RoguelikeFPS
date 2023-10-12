using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System;
using System.Linq;
using Unity.AI.Navigation;
using System.Linq.Expressions;

public class MapGenerator : MonoBehaviour {

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

    public MapDisplay display;
    
    public bool generateGeometryOnPlay;
    private bool playing;

    private void Start()
    {
        playing = true;
        display = GetComponent<MapDisplay>();
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

        for(int x = 0; x < mapSize; x++)
            for(int y = 0; y < mapSize; y++)
            {
                float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset + new Vector2(x * (chunkSize -1), y * (chunkSize -1)));

                display.DrawMesh(GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), new Vector3(x * (chunkSize -1), 0, y * (chunkSize -1)), new int2(x * mapSize + y, mapSize * mapSize), noiseMap);
            }
        level.Generate();
        for(int i = 0; i < display.chunks.Count; i++)
        {
            PreloadChunk(i);
            if(!playing || true) LoadChunk(i);
        }
        //foliage.GenerateMatrices();
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    public void PreloadChunk(int chunkId)
    {
        Transform chunk = display.chunks[chunkId].gameObject.transform;
        FoliageGenerator f = CopyComponent(foliage, chunk.gameObject);
        f.grassPositions = new List<List<Vector3>>();
        f.batches = new List<List<List<Matrix4x4>>>();
    }

    public IEnumerator LoadChunk(int chunkId)
    {
        Transform chunk = display.chunks[chunkId].gameObject.transform;

        Vector2 chunkPos = new Vector2(chunk.GetSiblingIndex() % mapSize, chunk.GetSiblingIndex() / mapSize);
        FoliageGenerator f = chunk.GetComponent<FoliageGenerator>();
        f.GenerateGrass(chunk, meshHeightMultiplier, meshHeightCurve);
        f.GenerateTrees(chunk, meshHeightMultiplier, meshHeightCurve, chunkPos);
        f.GenerateRocks(chunk, meshHeightMultiplier, meshHeightCurve);
        f.GenerateMatrices();

        //-----Foliage displcement is really unoptimized rn-----
        //FoliageDisplacementHandler d = CopyComponent(GetComponent<FoliageDisplacementHandler>(), chunk.gameObject);
        //d.foliage = f;
        //d.Start();

        MapDisplay.TerrainChunk c = display.chunks[chunkId];
        c.loaded = true;
        display.chunks[chunkId] = c;

        yield return null;
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

    public Vector2 ChunkCoordsAtPosition(int chunkId, Vector3 position)
    {
        MapDisplay.TerrainChunk chunk = display.chunks[chunkId];
        Vector3 cPos = chunk.gameObject.transform.position;
        if (chunk.height == null) return Vector2.zero;
        int l = chunk.height.GetLength(0);
        return new Vector2(position.x - (cPos.x - chunkSize / 2), (cPos.z + chunkSize / 2) - position.z) / chunkSize * l;
    }

    public float HeightAtPosition(int chunkId, Vector3 position)
    {
        MapDisplay.TerrainChunk chunk = display.chunks[chunkId];
        if (chunk.height == null) return 0;
        int l = chunk.height.GetLength(0);
        Vector2 pos = ChunkCoordsAtPosition(chunkId, position);


        int x = Mathf.FloorToInt(Math.Clamp(pos.x, 0, l-1));
        int y = Mathf.FloorToInt(Math.Clamp(pos.y, 0, l-1));

        float height = chunk.height[x, y];
        float dx = (pos.x - Mathf.Floor(pos.x)) * (chunk.height[Math.Clamp(x+1,0, l-1), y] - height);
        float dy = (pos.y - Mathf.Floor(pos.y)) * (chunk.height[x, Math.Clamp(y + 1, 0, l - 1)] - height);
        
        height += (dx + dy)/2f;

        return height * meshHeightMultiplier * meshHeightCurve.Evaluate(height);
    }

    public float SlopeAtPosition(int chunkId, Vector3 position)
    {
        MapDisplay.TerrainChunk chunk = display.chunks[chunkId];
        if (chunk.height == null) return 0;
        Vector2 pos = ChunkCoordsAtPosition(chunkId, position);

        int l = chunk.height.GetLength(0);

        int x = Mathf.FloorToInt(Math.Clamp(pos.x, 0, l-1));
        int y = Mathf.FloorToInt(Math.Clamp(pos.y, 0, l-1));
        float height = chunk.height[x, y];

        // Compute the differentials by stepping over 1 in both directions.
        float dx = chunk.height[x + (x+1 < l? 1 : -1), y] - height;
        float dy = chunk.height[x, y + (y + 1 < l ? 1 : -1)] - height;

        // The "steepness" is the magnitude of the gradient vector
        // For a faster but not as accurate computation, you can just use abs(dx) + abs(dy)
        float slope = Mathf.Abs(dx) + Mathf.Abs(dy);

        //print(slope);
        return slope * meshHeightMultiplier * meshHeightCurve.Evaluate(height);
        //return Mathf.Sqrt(dx * dx + dy * dy);
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
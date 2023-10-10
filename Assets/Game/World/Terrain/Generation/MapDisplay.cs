using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;
using static UnityEditor.PlayerSettings;

public class MapDisplay : MonoBehaviour {

    [SerializeField]
	public List<TerrainChunk> chunks;
	public Material material;

    public MapGenerator generator;
    public FoliageGenerator foliage;

    public void DrawTexture(Texture2D texture, int n)
    {
        //chunks[n].gameObject.GetComponent<MeshRenderer>().material = noiseMaterial;
        chunks[n].gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
    }

    public void DrawMesh(MeshData meshData, Vector3 pos, int2 ids, float[,] height) {
        Mesh mesh = meshData.CreateMesh();
        if (chunks.Count > ids.x)
        {
            if (chunks[0].gameObject == null || chunks[0].data == null || chunks[0].data.vertices.Length != meshData.vertices.Length)
            {
                ClearChunks();
                DrawMesh(meshData, pos, ids, height);
                return;
            }
            chunks[ids.x].gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
            chunks[ids.x].gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        else if (ids.x == ids.y)
        {
            ClearChunks(ids.y + 1);
        }
        else
        {
            GameObject go = new GameObject("Terrain Chunk " + ids.x);
            go.transform.SetParent(transform);
            go.transform.localPosition = pos;

            go.layer = 9;

            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshCollider>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().material = material;

            chunks.Add(new TerrainChunk(meshData, go));
        }
    }

    [System.Serializable]
   public struct TerrainChunk
    {
        public MeshData data;
        public GameObject gameObject;
 
        public TerrainChunk(MeshData m, GameObject g)
        {
            data = m;
            gameObject = g;
        }
    }

    public void ClearChunks(int start = 0)
    {
        for (int i = chunks.Count - 1; i >= start; i--)
        {
            if(chunks[i].gameObject != null) DestroyImmediate(chunks[i].gameObject);
            chunks.RemoveAt(i);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using UnityEngine.UIElements;

public class ChunkHandler : MonoBehaviour
{
    public Transform player;
    public int renderDistance;
    public int grassDistance;

    private MapDisplay mapDisplay;
    private MapGenerator mapGenerator;
    public Vector2Int pos;


    private void Awake()
    {
        UnityThread.initUnityThread();
    }

    private void Start()
    {
        mapDisplay = GetComponent<MapDisplay>();
        mapGenerator = GetComponent<MapGenerator>();
    }

    private void Update()
    {
        Vector2Int newPos = GetPlayerPos();
        if(pos != newPos)
        {
            pos = newPos;
            UpdateChunks();
        }
    }

    public Vector2Int GetPlayerPos()
    {
        Vector3 playerPos = player.position - transform.position + new Vector3(mapGenerator.chunkSize, 0, mapGenerator.chunkSize) / 2;
        return new Vector2Int((int)playerPos.x, (int)playerPos.z) / mapGenerator.chunkSize;
    }

    public void UpdateChunks()
    {
        if (!(-renderDistance < pos.x && pos.x < mapGenerator.mapSize + renderDistance && -renderDistance < pos.y && pos.y < mapGenerator.mapSize + renderDistance)) return;
        for(int x = 0; x < mapGenerator.mapSize; x++)
            for(int y = 0; y < mapGenerator.mapSize; y++)
            {
                int id = x * mapGenerator.mapSize + y;
                if (pos.x - renderDistance < x && pos.x + renderDistance > x && pos.y - renderDistance < y && pos.y + renderDistance > y)
                {
                    if (!mapDisplay.chunks[id].gameObject.activeSelf) mapDisplay.chunks[id].gameObject.SetActive(true);
                    if (!mapDisplay.chunks[id].loaded)
                    {
                        UnityThread.executeCoroutine(mapGenerator.LoadChunk(id));

                        //Thread chunkThread = new Thread(() => mapGenerator.LoadChunk(id));
                        //chunkThread.Start();
                        //mapGenerator.LoadChunk(id);
                    }
                    mapDisplay.chunks[id].gameObject.GetComponent<FoliageGenerator>().enabled = pos.x - grassDistance < x && pos.x + grassDistance > x && pos.y - grassDistance < y && pos.y + grassDistance > y;
                }
                else
                {
                    if (mapDisplay.chunks[id].gameObject.activeSelf) mapDisplay.chunks[id].gameObject.SetActive(false);
                }
            }
    }
}

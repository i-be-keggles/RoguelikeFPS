using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewItemSpawnMatrix", menuName = "Item Spawn Matrix")]
public class ItemSpawnMatrix : ScriptableObject
{
    public List<Item> items;

    public Item GetRandomItem()
    {
        int totalSpawnRatio = 0;
        foreach (Item item in items) totalSpawnRatio += item.spawnRatio;

        int n = Random.Range(0, totalSpawnRatio + 1);

        foreach (Item item in items)
        {
            n -= item.spawnRatio;
            if (n <= 0) return item;
        }

        throw new System.Exception("Item index out of range.");
    }

    [System.Serializable]
    public struct Item
    {
        public enum ItemType { weapon, throwable, consumable }

        public GameObject prefab;
        public int spawnRatio;
        public ItemType type;

        public Item(GameObject g, int r, ItemType t)
        {
            prefab = g;
            spawnRatio = r;
            type = t;
        }
    }
}

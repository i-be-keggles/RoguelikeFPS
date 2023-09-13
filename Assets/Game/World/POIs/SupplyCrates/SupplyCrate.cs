using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SupplyCrate : MonoBehaviour
{
    public ItemSpawnMatrix matrix;
    public Transform[] spawnPoints;

    public int crystal;
    private ScoreManager scoreManager;

    public enum SpawnBehaviour { spawnOnAwake, spawnOnInteract, spawnManually };
    public SpawnBehaviour spawnBehaviour;

    private bool spawned = false;

    public void Awake()
    {
        if (crystal > 0) scoreManager = FindObjectOfType<ScoreManager>();
        if (spawnBehaviour == SpawnBehaviour.spawnOnAwake) Spawn();
        else if (spawnBehaviour == SpawnBehaviour.spawnOnInteract)
        {
            GetComponent<Interactable>().interactedWith += OnInteracted;
        }
    }

    private void OnInteracted(object sender, EventArgs e)
    {
        if (crystal > 0) scoreManager.AddCrystal(crystal);
        Spawn();
        Destroy(GetComponent<Interactable>());
    }

    public void Spawn()
    {
        if (spawned) return;
        spawned = true;

        foreach(Transform t in spawnPoints)
        {
            ItemSpawnMatrix.Item item = matrix.GetRandomItem();
            GameObject go = Instantiate(item.prefab);
            Rigidbody rb = go.GetComponentInChildren<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            go.transform.position = t.position;
            go.transform.rotation = t.rotation;
            go.transform.SetParent(t, true);
        }
    }
}

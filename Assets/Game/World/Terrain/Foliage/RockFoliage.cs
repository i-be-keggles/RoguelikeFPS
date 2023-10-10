using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrassObject", menuName = "Foliage/Rock")]
public class RockFoliage : Foliage
{
    public GameObject[] mesh;

    public override GameObject GetPrefab()
    {
        int n = Random.Range(0, mesh.Length);
        return mesh[n];
    }
}
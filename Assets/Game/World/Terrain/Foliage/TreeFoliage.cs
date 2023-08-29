using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrassObject", menuName = "Foliage/Tree")]
public class TreeFoliage : Foliage
{
    public GameObject mesh;
    public float sizeVariation;

    public override GameObject GetPrefab()
    {
        return mesh;
    }
}
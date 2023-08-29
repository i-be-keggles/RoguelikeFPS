using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrassObject", menuName = "Foliage/Rock")]
public class RockFoliage : Foliage
{
    public GameObject mesh;

    public override GameObject GetPrefab()
    {
        return mesh;
    }
}
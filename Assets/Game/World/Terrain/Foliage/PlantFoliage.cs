using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantObject", menuName = "Foliage/Plant")]
public class PlantFoliage : Foliage
{
    public GameObject mesh;
    [Range(0, 2)] public int spawnPass;
}
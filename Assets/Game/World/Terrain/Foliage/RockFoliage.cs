using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrassObject", menuName = "Foliage/Rock")]
public class RockFoliage : Foliage
{
    public Mesh mesh;
    [Range(0, 2)] public int spawnPass;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Foliage : ScriptableObject
{
    public string name;
    [Range(0,1)] public float density;
    public float densityRadius;

    [Range(0,90)] public float slopeCutoff = 90;
    [Range(0, 1)] public float slopeAversion;

    [Min(0)] public float maxHeight = 100000;
    [Min(0)] public float minHeight;

    [Space]
    public Material material;
    public Vector3 baseRotation;
    public Vector3 baseOffset;

    [Space]
    [Tooltip("0 = no clusters.")]
    public int clusterSize;
    public float clusterDensity;
    public float clusterVariance;

    [Tooltip("Calculates probability of plant spawning at given steepness.")]
    public float PointProbability(float height, float angle)
    {
        if (angle >= slopeCutoff || height > maxHeight || height < minHeight) return 0;
        else return 1 - slopeAversion * (angle / slopeCutoff);
    }

    public int GetID()
    {
        Type t = GetType();
        return t == typeof(PlantFoliage) ? 0 : t == typeof(TreeFoliage) ? 1 : 2;
    }

    public virtual GameObject GetPrefab()
    {
        return null;
    }
}
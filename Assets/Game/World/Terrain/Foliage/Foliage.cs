using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Foliage : ScriptableObject
{
    public string name;
    [Range(0,1)] public float density;
    public float densityRadius;

    [Range(0,90)] public float slopeCutoff;
    [Range(0, 1)] public float slopeAversion;

    [Space]
    public Material material;
    public Vector3 baseRotation;
    public Vector3 baseOffset;


    [Tooltip("Calculates probability of plant spawning at given steepness.")]
    public float SlopeProbability(float angle)
    {
        if (angle >= slopeCutoff) return 0;
        else return 1 - slopeAversion * (angle / slopeCutoff);
    }

    public int GetID()
    {
        Type t = GetType();
        return t == typeof(PlantFoliage) ? 0 : t == typeof(TreeFoliage) ? 1 : 2;
    }
}
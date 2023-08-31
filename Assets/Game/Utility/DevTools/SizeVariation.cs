using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SizeVariation
{
    public static float RandomSize(float u, float s, bool clamp0=true)
    {
        float n = u + u * Random.Range(-s, s);
        if (clamp0) return Mathf.Max(0, n);
        else return n;
    }

    public static int RandomSizeInt(float u, float s, bool clamp0 = true)
    {
        float n = u + u * Random.Range(-s, s);
        if (clamp0) return Mathf.RoundToInt(Mathf.Max(0, n));
        else return Mathf.RoundToInt(n);
    }
}

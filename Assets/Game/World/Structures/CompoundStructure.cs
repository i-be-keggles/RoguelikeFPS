using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CompoundStructure : MonoBehaviour
{
    public enum CompoundStructureType { hub, connector }

    public string name;

    public int size; //corresponding to eg. [10m, 20m, 30m]

    public int maxPresent; //max number of instances in a compound
    public int minPresent;

    public Vector3[] connectionPoints;
    public List<CompoundStructure> connectedStructures;

    private void Start()
    {
        connectedStructures = new List<CompoundStructure>(connectionPoints.Length);
    }
}
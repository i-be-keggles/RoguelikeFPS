using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CompoundGenerator : Monobehaviour
{
    int size; //nhubs
    [Range(1,2)] float bufferSpace; //consider hubs x times bigger

    //change as necesseray to fit models
    public int maxHubSize = 3;
    public int hubSizeIncrement = 15; //radius
    public int maxConnectorSize = 3;
    public int connecterSizeIncrement = 10;

    public GameObject[] g_connectors;
    public GameObject[] g_hubs;

    private List<CompoundStructure> m_connectors;
    private List<CompoundStructure> m_hubs;

    private List<CompoundStructure> connectors;
    private List<CompoundStructure> hubs;


    public void Start()
    {
        m_connectors = new List<CompoundStructure>();
        foreach (GameObject g in g_connectors) m_connectors.Add(g.GetComponent<CompoundStructure>());

        m_hubs = new List<CompoundStructure>();
        foreach (GameObject g in g_hubs) m_hubs.Add(g.GetComponent<CompoundStructure>());

        Generate();
    }

    public void Generate()
    {
        List<CompoundStructure> p_hubs = null;
        List<CompoundStrcutre> p_connectors = null;
        int n = -1;

        //spawn initial size 3
        while (n == -1)
        {
            n = [UnityEngine.Random.Range(m_hubs.Count);
            p_hubs = new List<CompoundStructure>() { m_hubs[n].GetComponent<CompoundStructure>() };
            if (p_hubs[0].size != maxHubSize) n = -1;
        }

        CompoundStructure hub = Instantiate(g_hubs[n], transform).GetComponent<CompoundStructure>();
        hubs.Add(hub);

        //spawn individuals
        while(hubs.Count < size)
        {
            if(p_hubs.Count == 0) p_hubs.Add(hubs.LastIndexOf());
            for (int i = 0; i < p_hubs.Count; i++)
            {
                for (int j = 0; j < p_hubs[i].connectionPoints.Size; j++)
                {
                    float desiredConnectors = 3;
                    desiredConnectors = Math.Min(desiredConnectors, p_hubs[i].connectionPoints.Length);
                    if(p_hubs[i].connectedStructures[j] == null && UnityEngine.Random.random(/*0f-1f*/) > desiredConnectors / p_hubs[i].connectionPoints.Length)
                    {
                        //spawn connector
                        n = m_connectors.IndexOf(GetConnecter());
                        CompoundStructure connector = Instantiate(g_connectors[n], transform).GetComponent<CompoundStructure>();
                        connectors.Add(connector);
                        p_connectors.Add(connector);
                    }
                }
            }
            if (p_connectors.Count == 0) continue;

            hub = GetHub();
            n = m_hubs.IndexOf(hub)
        }
    }

    public CompoundStructure GetHub(int connector)
    {
        CompoundStructure c = connectors[connector];
        Vector3 c = c.connectedStructures[0] == null ? c.connectionPoints[0 : 1];

        int maxSize = maxHubSize; //change as necessary
        for(maxSize; maxSize >= 0; maxSize--)
        {
            if (maxSize == 0) return null;

            int r = hubSizeIncrement * maxSize;

            bool fits = true;

            for (int i = 0; i < hubs.Count; i++)
            {
                if (Vector3.Distance(point, hubs[i].position) < hubSizeIncrement * bufferSpace * maxSize + hubSizeIncrement * bufferSpace * hubs[i].size)
                {
                    fits = false;
                    break;
                }
            }

            if (fits) break;
        }

        List<CompoundStructure> s_hubs;
        foreach(CompoundStructure hub in m_hubs) if (hub.size <= maxSize) s_hubs.Add(hub);

        CompoundStructure hub = null;
        while (hub == null)
        {
            hub = s_hubs[UnityEngine.Random.Range(s_hubs.Count)];

            if(hub.maxPresent > 0)
            {
                int present = 0;

                foreach (CompoundStructure h in hubs)
                {
                    if (hub.name.equals(h.name)) present++;
                }

                if (present > hub.maxPresent) hub = null;
            }
        }

        return hub;
    }

    public CompoundStructure GetConnecter(float size = 0)
    {
        List<CompoundStructure> p_connectors = new List<CompoundStructure>();

        size = Mathf.clamp(size, 0, maxConnectorSize);

        foreach(CompoundStructure c in m_connectors) if(size == 0 || size == c.size) p_connecetors.Add(c);
    }

    public void DeleteConnector(int n)
    {
        CompoundStructre c = connectors[n]);
        connectors.Remove(c);

        for(int i = hubs.Count - 1; i >= 0; i--)
        {
            if (hubs[i].connectedStructures.Contains(c))
            {
                hubs[i].connectedStructures.Remove(c);
                break;
            }
        }
    }

    //place mandatory hub structures
    public void EnforceMins()
    {

    }
}
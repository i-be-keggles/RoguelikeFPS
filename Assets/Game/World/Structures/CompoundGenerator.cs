using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CompoundGenerator : MonoBehaviour
{
    [Min(1)] public int size = 10; //nhubs
    [Range(1,2)] public float bufferSpace = 1.5f; //consider hubs x times bigger

    //change as necesseray to fit models
    public int maxHubSize = 3;
    public float hubSizeIncrement = 15; //radius
    public int maxConnectorSize = 3;
    public float connectorSizeIncrement = 10;

    public GameObject[] g_connectors;
    public GameObject[] g_hubs;

    private List<CompoundStructure> m_connectors;
    private List<CompoundStructure> m_hubs;

    private List<CompoundStructure> connectors;
    private List<CompoundStructure> hubs;

    public float scale = 0.5f;
    public float yOffset = 5f;

    private UnityTerrainGenerator terrainGen;

    public void Init()
    {
        terrainGen = GetComponentInParent<UnityTerrainGenerator>();

        m_connectors = new List<CompoundStructure>();
        foreach (GameObject g in g_connectors) m_connectors.Add(g.GetComponent<CompoundStructure>());

        m_hubs = new List<CompoundStructure>();
        foreach (GameObject g in g_hubs) m_hubs.Add(g.GetComponent<CompoundStructure>());

        Generate();
    }

    public void Generate()
    {
        List<CompoundStructure> p_hubs;
        List<CompoundStructure> p_connectors;
        hubs = new List<CompoundStructure>();
        connectors = new List<CompoundStructure>();
        int n = -1;

        //spawn initial size 3
        while (n == -1)
        {
            n = UnityEngine.Random.Range(0, m_hubs.Count);
            p_hubs = new List<CompoundStructure>() { m_hubs[n] };
            if (p_hubs[0].size != maxHubSize) n = -1;
        }

        CompoundStructure hub = Instantiate(g_hubs[n], transform).GetComponent<CompoundStructure>();
        hub.transform.eulerAngles = new Vector3(0, UnityEngine.Random.Range(0f, 360f));
        hubs.Add(hub);
        p_hubs = new List<CompoundStructure>() { hub };

        int t = 0;
        //spawn individuals
        while(hubs.Count < size)
        {
            t++;
            if(t > size * 10)
            {
                Debug.LogError("Generation failed.");
                return;
            }


            if(p_hubs.Count == 0) p_hubs.Add(hubs[hubs.Count-1]);

            p_connectors = new List<CompoundStructure>();

            for (int i = 0; i < p_hubs.Count; i++)
            {
                for (int j = 0; j < p_hubs[i].connectionPoints.Length; j++)
                {
                    float desiredConnectors = 3;
                    desiredConnectors = Math.Min(desiredConnectors, p_hubs[i].connectionPoints.Length);
                    if(p_hubs[i].connectedStructures[j] == null && UnityEngine.Random.Range(0f, 1f) < desiredConnectors / p_hubs[i].connectionPoints.Length)
                    {
                        //spawn connector
                        n = m_connectors.IndexOf(GetConnecter());
                        CompoundStructure connector = Instantiate(g_connectors[n], transform).GetComponent<CompoundStructure>();
                        connectors.Add(connector);
                        p_connectors.Add(connector);

                        p_hubs[i].connectedStructures[j] = connector;
                        connector.connectedStructures[0] = p_hubs[i];

                        Vector3 dir = (p_hubs[i].transform.TransformPoint(p_hubs[i].connectionPoints[j]) - p_hubs[i].transform.position).normalized;
                        dir.y = 0;

                        connector.transform.position = p_hubs[i].transform.position + dir * (p_hubs[i].size * hubSizeIncrement + connector.size * connectorSizeIncrement);
                        connector.transform.eulerAngles = Quaternion.LookRotation(p_hubs[i].transform.position - connector.transform.position).eulerAngles + new Vector3(0, -90, 0);
                    }
                }
            }
            if (p_connectors.Count == 0) continue;

            p_hubs = new List<CompoundStructure>();
            for(int i = 0; i < p_connectors.Count; i++)
            {
                hub = GetHub(connectors.IndexOf(p_connectors[i]));
                if(hub == null)
                {
                    DeleteConnector(connectors.IndexOf(p_connectors[i]));
                    print("Deleting connector");
                    continue;
                }
                n = m_hubs.IndexOf(hub);

                hub = Instantiate(g_hubs[n], transform).GetComponent<CompoundStructure>();
                hubs.Add(hub);
                p_hubs.Add(hub);

                p_connectors[i].connectedStructures[1] = hub;
                hub.connectedStructures[0] = p_connectors[i];

                hub.transform.position = p_connectors[i].transform.position + -p_connectors[i].transform.right * (p_connectors[i].size * connectorSizeIncrement + hub.size * hubSizeIncrement);
                //hub.transform.eulerAngles = new Vector3(0, -Vector3.SignedAngle(hub.transform.TransformPoint(hub.connectionPoints[0]) - hub.transform.position, p_connectors[i].transform.position - transform.position, Vector3.up));
                hub.transform.eulerAngles = Quaternion.LookRotation(p_connectors[i].transform.position - transform.position).eulerAngles + new Vector3(0,+90,0);
            }
        }

        transform.localScale *= scale;
        transform.position += new Vector3(0, yOffset, 0);

        //flatten terrain
        foreach (CompoundStructure h in hubs)
            terrainGen.FlattenArea(h.size * hubSizeIncrement * scale * 1.25f, h.transform.position - new Vector3(0, yOffset/2, 0));
    }

    public CompoundStructure GetHub(int connector)
    {
        CompoundStructure c = connectors[connector];
        Vector3 point = c.transform.TransformPoint(c.connectionPoints[c.connectedStructures[0] == null ? 0 : 1]);

        int maxSize; //change as necessary
        for(maxSize = maxHubSize; maxSize >= 0; maxSize--)
        {
            if (maxSize == 0) return null;

            float r = hubSizeIncrement * maxSize;

            bool fits = true;
            Vector3 p = c.transform.position + (point - c.transform.position).normalized * (c.size * connectorSizeIncrement + r);

            for (int i = 0; i < hubs.Count; i++)
            {
                if (Vector3.Distance(p, hubs[i].transform.position) < r * bufferSpace + hubSizeIncrement * bufferSpace * hubs[i].size)
                {
                    fits = false;
                    break;
                }
            }

            if (fits) break;
        }

        List<CompoundStructure> s_hubs = new List<CompoundStructure>();
        foreach(CompoundStructure h in m_hubs) if (h.size <= maxSize) s_hubs.Add(h);

        CompoundStructure hub = null;
        while (hub == null)
        {
            hub = s_hubs[UnityEngine.Random.Range(0, s_hubs.Count)];

            if(hub.maxPresent > 0)
            {
                int present = 0;

                foreach (CompoundStructure h in hubs)
                {
                    if (hub.name.Equals(h.name)) present++;
                }

                if (present > hub.maxPresent) hub = null;
            }
        }

        return hub;
    }

    public CompoundStructure GetConnecter(float size = 0)
    {
        List<CompoundStructure> p_connectors = new List<CompoundStructure>();

        size = Mathf.Clamp(size, 0, maxConnectorSize);

        foreach(CompoundStructure c in m_connectors) if(size == 0 || size == c.size) p_connectors.Add(c);

        return p_connectors[UnityEngine.Random.Range(0, p_connectors.Count)];
    }

    public void DeleteConnector(int n)
    {
        CompoundStructure c = connectors[n];
        connectors.Remove(c);

        for(int i = hubs.Count - 1; i >= 0; i--)
        {
            if (hubs[i].connectedStructures.Contains(c))
            {
                hubs[i].connectedStructures.Remove(c);
                break;
            }
        }

        Destroy(c.gameObject);
    }

    //place mandatory hub structures
    public void EnforceMins()
    {

    }
}
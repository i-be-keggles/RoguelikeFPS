using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageDisplacementHandler : MonoBehaviour
{
    public Texture2D displacement;
    public MapGenerator map;
    public FoliageGenerator foliage;
    public int resolution = 1024;

    public float springiness;

    public PlayerMovement player;


    public void Start()
    {
        displacement = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
        for(int x = 0; x < resolution; x++)
            for(int y = 0; y < resolution; y++)
            {
                displacement.SetPixel(x, y, new Color(0, 0, 0));
            }

        foliage.grassDisplacementMap = displacement;
    }

    private void Update()
    {
        if (player.grounded) Impact(player.transform.position, 0.5f);

        //return;
        Color[] pixels = displacement.GetPixels();
        for(int i = 0; i < pixels.Length; i++)
        {
            pixels[i] /= 1 + springiness * Time.deltaTime;
        }
        displacement.SetPixels(pixels);
        displacement.Apply();
        
        foliage.grassDisplacementMap = displacement;
    }

    public void Impact(Vector3 position, float radius)
    {
        radius = radius / map.chunkSize * resolution;
        Vector2 pos = new Vector2(position.x - (transform.position.x - map.chunkSize / 2), position.z - (transform.position.z - map.chunkSize / 2)) / map.chunkSize * resolution;
        for (int x = Mathf.Max(Mathf.FloorToInt(pos.x - radius) - 1, 0); x < Mathf.Min(Mathf.FloorToInt(pos.x + radius) + 1, resolution); x++)
            for (int y = Mathf.Max(Mathf.FloorToInt(pos.y - radius) - 1,0); y < Mathf.Min(Mathf.FloorToInt(pos.y + radius) + 1, resolution); y++)
            {
                float dx = x - pos.x;
                float dy = y - pos.y;
                float d = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
                float m = Explosion.GetFalloff(radius-d, radius);
                float r = 1 - Mathf.Abs(dy/radius);
                float b = 1 - Mathf.Abs(dx/radius);
                displacement.SetPixel(x, y, new Color(r,dx > 0? 0 : 1, b, dy > 0? 0 : 1));
            }
        displacement.Apply();
    }
}

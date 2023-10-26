using UnityEngine;

public static class HeightmapUtil
{
    //height in [0,1], r as # of pixels
    public static float[,] NormaliseToHeight(float[,] map, Vector2 pos, float height, float r)
    {
        int s = map.GetLength(0);
        pos = new Vector2(s / 2f, s / 2f);

        for (int x = 0; x < map.GetLength(0); x++)
            for(int y = 0; y < map.GetLength(1); y++)
            {
                //distance from center
                float d = Mathf.Sqrt(Mathf.Pow(pos.x - x, 2) + Mathf.Pow(pos.y - y, 2)) - r;
                //influence of height
                float i = 1f / (1f + Mathf.Exp(d));
                //newHeight
                float h = map[x, y] * (1 - i) + height * i;
                map[x, y] = h;
            }
        return map;
    }
}
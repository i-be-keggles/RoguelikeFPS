using UnityEngine;
using System.Collections;

public static class Noise {

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, AnimationCurve curve = null, float falloff = Mathf.Infinity) {
		float[,] noiseMap = new float[mapWidth,mapHeight];

        float amplitude = 1;
        float frequency = 1;
        float maxPossibleHeight = 0;

		System.Random prng = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) - offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= persistance;
		}

		if (scale <= 0) {
			scale = 0.0001f;
		}

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
		
				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					float sampleX = (x-halfWidth + octaveOffsets[i].x) / scale * frequency * frequency;
					float sampleY = (y-halfHeight + octaveOffsets[i].y) / scale * frequency * frequency;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				noiseMap [x, y] = noiseHeight * (curve == null? 1 : curve.Evaluate(noiseHeight));
			}
		}

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
                noiseMap[x, y] = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight / 1.75f);
				
				//falloff
				float dx = Mathf.Abs(x - halfWidth)/ halfWidth;
				float dy = Mathf.Abs(y - halfWidth)/ halfWidth;
				float d = (1 - Mathf.Pow(dx, 2)) * (1 - Mathf.Pow(dy, 2));

                noiseMap[x, y] *= 1/(1+ Mathf.Exp(falloff*(0.25f-d)));


                //noiseMap[x, y] = 1 / (1 + Mathf.Exp(noiseMap[x, y] * 3));
            }
		}

		return noiseMap;
	}

}

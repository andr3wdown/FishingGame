using UnityEngine;

namespace PlanetGenerator2D
{
	public static class Noise
	{
		//Thanks to Sebastian League for his tutorial on the topic on youtube!!

		public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
		{
			float[,] noiseMap = new float[mapWidth, mapHeight];

			System.Random prng = new System.Random(seed);
			Vector2[] octaveOffsets = new Vector2[octaves];
			for (int i = 0; i < octaves; i++)
			{
				float offsetX = prng.Next(-100000, 100000) + offset.x;
				float offsetY = prng.Next(-100000, 100000) + offset.y;
				octaveOffsets[i] = new Vector2(offsetX, offsetY);
			}

			if (scale <= 0)
			{
				scale = 0.00001f;
			}

			float maxNoiseHeight = float.MinValue;
			float minNoiseHeight = float.MaxValue;

			float halfWidth = mapWidth / 2f;
			float halfHeight = mapHeight / 2f;

			for (int y = 0; y < mapHeight; y++)
			{
				for (int x = 0; x < mapWidth; x++)
				{
					float amplitude = 1;
					float frequency = 1;
					float noiseHeight = 0;


					for (int i = 0; i < octaves; i++)
					{
						float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
						float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

						float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
						noiseHeight += perlinValue * amplitude;

						amplitude *= persistance;
						frequency *= lacunarity;
					}

					if (noiseHeight > maxNoiseHeight)
					{
						maxNoiseHeight = noiseHeight;
					}
					else if (noiseHeight < minNoiseHeight)
					{
						minNoiseHeight = noiseHeight;
					}

					noiseMap[x, y] = noiseHeight;
					noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
				}
			}


			return noiseMap;
		}
	}
}
[System.Serializable]
public class NoiseProperties
{
	[Range(0f, 1f)]
	public float noiseWaterRatio = 0.5f;
	public int seed;
	[Range(0f, 100f)]
	public float scale;
	[Range(0f,1f)]
	public float persistence;
	[Range(0f, 5f)]
	public float lacunarity;
	[Range(1, 15)]
	public int octaves;
}


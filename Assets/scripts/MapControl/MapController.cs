using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using Andr3wDown.Math;

public class MapController : MonoBehaviour
{
	public const float MAX_TEMP = 27f;
	public float airTemperature;
	public float depth;
	public Tilemap ground;
	public Tilemap water;
	public TileBase waterTile;
	public TileBase groundTile;
	public TileBase stableWaterTile;
	public BoundsInt bounds;
	public int birthLimit;
	public int deathLimit;
	List<Vector2> waterTiles = new List<Vector2>();
	List<Vector2> groundTiles = new List<Vector2>();
	public MapMode mode;
	[Range(0f, 1f)]
	public float waterChance = 0.5f;
	[Range(0f, 1f)]
	public float stabilityTreshold;
	public Texture2D mapTexture;
	public bool perlinNoise = false;
	public bool loadFromTexture;
	public bool textureRandomness;
	public Color shallow;
	public Color deep;
	public Transform player;
	public int automataIterations = 3;
	public BoundsInt automataBounds;
	[Header("NoiseGen Controls")]
	public NoiseProperties noiseProperties;
	static MapController controller;

	[Header("Testing Fish Locales")]
	public FishInformation[] testFish;
	public bool includeTempInfo;
	public void Start()
	{
		controller = this;
		Generate();
	}
	Dictionary<Vector3Int, CellData> depthChart = new Dictionary<Vector3Int, CellData>();
	public static bool IsOnWater(Vector2 pos)
	{
		return controller.water.GetTile(controller.water.WorldToCell(pos)) != null;
	}
	public static float GetDepth(Vector2 position)
	{
		Vector3Int gridPos = controller.water.WorldToCell(position);
		if (controller.depthChart.ContainsKey(gridPos))
		{
			CellData val = new CellData();
			controller.depthChart.TryGetValue(gridPos, out val);
			return val.depth;
		}
		return 0;
	}
	public static float GetTemp(Vector2 position, float depth)
	{
		Vector3Int gridPos = controller.water.WorldToCell(position);
		if (controller.depthChart.ContainsKey(gridPos))
		{
			CellData val = new CellData();
			controller.depthChart.TryGetValue(gridPos, out val);
			return val.GetTemp(depth);
		}
		return 0;
	}
	public static Vector3Int GetGridPos(Vector2 position)
	{
		Vector3Int gridPos = controller.water.WorldToCell(position);
		return gridPos;
	}
	public static float Depth
	{
		get
		{
			return controller.depth;
		}
	}
	public static float Temperature
	{
		get
		{
			return controller.airTemperature;
		}
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			Generate();
		}
	}
	int[,] map;
	int[] dismap;
	public void Clear()
	{
		water.ClearAllTiles();
		ground.ClearAllTiles();
		waterTiles = new List<Vector2>();
		groundTiles = new List<Vector2>();
		depthChart = new Dictionary<Vector3Int, CellData>();
	}
	public Dictionary<Vector3Int, CellData> Generate()
	{
		if(controller == null) controller = this;
		Clear();
		if (perlinNoise) return GeneratePerlinNoise();	
		if (loadFromTexture) bounds.size = new Vector3Int(mapTexture.width, mapTexture.height, 1);

		width = bounds.size.x;
		height = bounds.size.y;
		bounds.position = new Vector3Int(-bounds.size.x / 2, -bounds.size.y / 2, 0);
		TileBase[] tiles = water.GetTilesBlock(bounds);
		TileBase[] gtiles = ground.GetTilesBlock(bounds);

		if (!loadFromTexture)
		{
			map = Generate(bounds.size.x, bounds.size.y);
			dismap = DismantleMap(map);
		}
		else
		{
			dismap = GetMapFromTexture();
			if (textureRandomness)
			{
				dismap = Generate(dismap);
			}
		}


		print($"{dismap.Length} {tiles.Length}");
		
	
		for (int i = 0; i < tiles.Length; i++)
		{
			if (dismap[i] == 1)
			{
				tiles[i] = waterTile;
				//print($"{i % height} {i / height}");
				waterTiles.Add(water.CellToWorld(new Vector3Int(i % height - height / 2, i / height - height / 2, 0)));
			}
			else
			{
				gtiles[i] = groundTile;
				groundTiles.Add(ground.CellToWorld(new Vector3Int(i % height - height / 2, i / height - height / 2, 0)));
			}
		}
		water.SetTilesBlock(bounds, tiles);
		ground.SetTilesBlock(bounds, gtiles);

		float min = float.MaxValue;
		float max = float.MinValue;
		float[] values = new float[waterTiles.Count];
		for(int i = 0; i < waterTiles.Count; i++)
		{
			//print(water.GetTile(water.WorldToCell(waterTiles[i])));
			if(water.GetTile(water.WorldToCell(waterTiles[i])) == waterTile)
			{
				float closestDst = float.MaxValue;
				for (int j = 0; j < groundTiles.Count; j++)
				{
					if (ground.GetTile(ground.WorldToCell(groundTiles[j])) == groundTile)
					{
						if (Vector2.Distance(waterTiles[i], groundTiles[j]) < closestDst)
						{
							closestDst = Vector2.Distance(waterTiles[i], groundTiles[j]);		
						}
					}
				}		
				if (closestDst < min)
				{
					min = closestDst;
				}
				if (closestDst > max)
				{
					max = closestDst;
				}
				values[i] = closestDst;				
			}
			
		}
		float[,] noiseMap = PlanetGenerator2D.Noise.GenerateNoiseMap(width, height, noiseProperties.seed, noiseProperties.scale, noiseProperties.octaves, noiseProperties.persistence, noiseProperties.lacunarity, new Vector2(2000, 2000));
		for (int i = 0; i < waterTiles.Count; i++)
		{
			if (water.GetTile(water.WorldToCell(waterTiles[i])) == waterTile || water.GetTile(water.WorldToCell(waterTiles[i])) == stableWaterTile)
			{
				float ratio = MathOperations.Map(values[i], min, max, 0f, 1f);
				Vector3Int cellPos = water.WorldToCell(waterTiles[i]);
				ratio = (noiseMap[cellPos.x + width / 2, cellPos.y + height / 2] * ratio) + (ratio * (1f - ratio));
				//0.3f+Mathf.PerlinNoise((waterTiles[i].x + (width / 2f)) * 0.11f, (waterTiles[i].y + (height / 2f)) * 0.11f);
				if(ratio > stabilityTreshold)
				{
					water.SetTile(water.WorldToCell(waterTiles[i]), stableWaterTile);
				}
				depthChart.Add(water.WorldToCell(waterTiles[i]), new CellData(ratio * depth));
				water.SetColor(water.WorldToCell(waterTiles[i]), Color.Lerp(shallow, deep, ratio));
			}

		}

		print($"min {min} max {max}");
		water.RefreshAllTiles();
		ground.RefreshAllTiles();
		player.position = (Vector2)waterTiles[Random.Range(0, waterTiles.Count)] + Vector2.one * 0.5f;
		transform.GetChild(0).GetComponent<TilemapRenderer>().sortingOrder = 0;
		transform.GetChild(1).GetComponent<TilemapRenderer>().sortingOrder = 0;

		return depthChart;
	}
	int[] GetMapFromTexture()
	{
		width = mapTexture.width;
		height = mapTexture.height;
		Color[] colors = mapTexture.GetPixels();
		int[] tiles = new int[colors.Length];
		for(int i = 0; i < colors.Length; i++)
		{
			if(colors[i] == Color.black)
			{
				tiles[i] = 1;
			}
			else
			{
				tiles[i] = 0;
			}
		}
		return tiles;
	}
	public int[,] Generate(int sizeX, int sizeY)
	{
		int[,] map = new int[sizeX, sizeY];
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				if(map[x,y] == 0)
					map[x, y] = (Random.Range(0f, 1f) < waterChance ? 1 : 0);
			}
		}
		map = Automata(map, automataIterations);
		return map;
	}
	public int[] Generate(int[] loadedMap)
	{
		map = ArrToMatrix(loadedMap);
		map = Automata(map, automataIterations);
		return DismantleMap(map);
	}
	int [,] ArrToMatrix(int[] arr, bool randomize = true)
	{
		int[,] matrix = new int[width, height];
		for(int i = 0; i < arr.Length; i++)
		{
			if (randomize)
			{
				if(arr[i] == 0)
				{
					arr[i] = (Random.Range(0f, 1f) < waterChance ? 1 : 0);
				}
			}
			int x = i / width;
			int y = i % width;
			matrix[x, y] = arr[i];
		}
		return matrix;
	}
	public int[] DismantleMap(int[,] map)
	{
		List<int> holder = new List<int>();
		for (int x = 0; x < map.GetLength(0); x++)
		{
			for (int y = 0; y < map.GetLength(1); y++)
			{
				holder.Add(map[x, y]);
			}
		}
		return holder.ToArray();
	}
	int width;
	int height;
	public int[,] Automata(int[,] old, int iterations=3)
	{
	
		for(int i = 0; i < iterations; i++)
		{
			old = RunIter(old, birthLimit, deathLimit);
		}
		return old;
	}

	int[,] RunIter(int[,] old, int br, int de)
	{
		int[,] nMap = new int[width, height];
		int n = 0;
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				n = 0;
				foreach(var b in automataBounds.allPositionsWithin)
				{
					if (b.x == 0 && b.y == 0) continue;
					if(x+b.x >= 0 && x+b.x < width && y+b.y >= 0 && y+b.y < height)
					{
						n += old[x + b.x, y + b.y];
					}
					else
					{
						if (mode == MapMode.lake)
						{
							n--;
						}
						else if(mode == MapMode.sea)
						{
							n++;
						}
						else if(mode == MapMode.river)
						{
							if(old[x, y] == 1)
							{
								n++;
							}
							else
							{
								n--;
							}
						}
					}
				}
				if(old[x,y] == 1)
				{
					if (n < de) nMap[x, y] = 0;
					else nMap[x, y] = 1;
				}
				if (old[x, y] == 0)
				{
					if (n > br) nMap[x, y] = 1;
					else nMap[x, y] = 0;
				}
			}
		}
		return nMap;
	}
	public Dictionary<Vector3Int, CellData> GeneratePerlinNoise()
	{
		width = bounds.size.x;
		height = bounds.size.y;
		bounds.position = new Vector3Int(-bounds.size.x / 2, -bounds.size.y / 2, 0);
		TileBase[] tiles = water.GetTilesBlock(bounds);
		TileBase[] gtiles = ground.GetTilesBlock(bounds);

		float[,] noiseMap = PlanetGenerator2D.Noise.GenerateNoiseMap(width, height, noiseProperties.seed, noiseProperties.scale, noiseProperties.octaves, noiseProperties.persistence,  noiseProperties.lacunarity, new Vector2(2000, 2000));
		int[,] tileMap = new int[width, height];
		List<int> map = new List<int>();
		for (int x = 0; x < noiseMap.GetLength(0); x++)
		{
			for (int y = 0; y < noiseMap.GetLength(1); y++)
			{
				bool isWater = noiseMap[x, y] > noiseProperties.noiseWaterRatio;
			
				tileMap[x, y] =  isWater ? 1 : 0;
				map.Add(tileMap[x, y]);
				if (isWater)
				{
					water.SetTile(new Vector3Int(x - width / 2, y - height / 2, 0), waterTile);
					waterTiles.Add(water.CellToWorld(new Vector3Int(x - width / 2, y - height / 2, 0)));
					float ratio = MathOperations.Map(Mathf.Clamp(noiseMap[x, y], noiseProperties.noiseWaterRatio, 1f), noiseProperties.noiseWaterRatio, 1f, 0f, 1f);
					if (ratio > stabilityTreshold)
					{
						water.SetTile(new Vector3Int(x - width / 2, y - height / 2, 0), stableWaterTile);
					}
					depthChart.Add(new Vector3Int(x - width / 2, y - height / 2, 0), new CellData(Mathf.Clamp(ratio, 0f, 1f) * depth));
					water.SetColor(new Vector3Int(x - width / 2, y - height / 2, 0), Color.Lerp(shallow, deep, Mathf.Clamp(ratio, 0f, 1f)));
				}
				else
				{
					ground.SetTile(new Vector3Int(x - width / 2, y - height / 2, 0), groundTile);
					groundTiles.Add(water.CellToWorld(new Vector3Int(x - width / 2, y - height / 2, 0)));
				}
			}
				
		}
		dismap = map.ToArray();

		water.RefreshAllTiles();
		player.position = (Vector2)waterTiles[Random.Range(0, waterTiles.Count)] + Vector2.one * 0.5f;
		return depthChart;
	}
	public void GenerateFishBiteMap()
	{

	}
}
public struct CellData
{
	public float depth;
	public float topTemp;
	public float botTemp;
	
	public CellData(float depth=0)
	{
		this.depth = depth;
		topTemp = MapController.Temperature * (1f - (0.1f * (depth / MapController.Depth)));
		botTemp = topTemp - depth;
		botTemp = Mathf.Clamp(botTemp, 4, float.MaxValue);
	}
	public float GetTemp(float currentDepth)
	{
		return Mathf.Lerp(topTemp, botTemp, currentDepth / depth);
	}
}
public enum MapMode
{
	sea,
	lake,
	river
}

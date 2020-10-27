using System;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColourMap, Mesh, FallOffMap};
    public DrawMode drawMode;
    public Noise.NormaliseMode normaliseMode;
    public const int chunkMapSize = 95;
    public bool useFlatShading;
    [Range(0, 6)]
    public int previewLOD;
    public float scale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public bool useFallOff;
    public bool autoUpdate;
    public Vector2 offset;
    public TerrainType[] regions;
    static MapGenerator instance;
    float[,] falloffMap;
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
    private void Awake()
    {
        falloffMap = FallOffMap.GenerateFallOffMap(chunkMapSize);
    }
    public static int mapChunkSize
    {
        get
        {
            if(instance==null)
            {
                instance = FindObjectOfType<MapGenerator>();
            }
            if(instance.useFlatShading)
            {
                return 95;
            }
            else
            {
                return 239;
            }
        }
    }
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap) 
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colorMap, chunkMapSize, chunkMapSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, previewLOD, useFlatShading), TextureGenerator.TextureFromColourMap(mapData.colorMap, chunkMapSize, chunkMapSize));
        }
        else if(drawMode == DrawMode.FallOffMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOffMap.GenerateFallOffMap(chunkMapSize)));
        }
    }
    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(centre, callback);
        };
        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };
        new Thread(threadStart).Start();
    }
    public void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod, useFlatShading);
        lock(meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i< mapDataThreadInfoQueue.Count; i++)
            {
                lock (mapDataThreadInfoQueue)
                {
                    MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }
        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                lock (meshDataThreadInfoQueue)
                {
                    MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }
    }
    MapData GenerateMapData(Vector2 centre)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(chunkMapSize + 2, chunkMapSize + 2, scale, octaves, persistance, lacunarity, seed, centre + offset, normaliseMode);
        Color[] colourMap = new Color[chunkMapSize * chunkMapSize];
        for(int y = 0; y<chunkMapSize; y++)
        {
            for(int x=0; x<chunkMapSize; x++)
            {
                if(useFallOff)
                {
                    noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y] - falloffMap[x, y], 0f , 1f);
                }
                float currentHeight = noiseMap[x, y];
                for(int i =0; i<regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * chunkMapSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colourMap);
        

    }
    void OnValidate()
    {
        if (scale <= 0)
        {
            scale = 0.01f;
        }
        if (octaves <= 0)
        {
            octaves = 1;
        }
        else if (octaves >= 22)
        {
            octaves = 21;
        }
        if (persistance <= 0)
        {
            persistance = 0.01f;
        }
        if (lacunarity <= 0)
        {
            lacunarity = 0.01f;
        }
        falloffMap = FallOffMap.GenerateFallOffMap(chunkMapSize);
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}

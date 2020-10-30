using UnityEngine;
using System;
using System.Runtime.InteropServices;

public static class Noise
{
    public enum NormalizeMode { Local, Global };
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int octaves, float persistance, float lacunarity, float scale, int seed, Vector2 offset, NormalizeMode normaliseMode)
    {
        float sampleX;
        float sampleY;
        float noisePoint;

        float minLocalNoiseHeight = float.MaxValue;
        float maxLocalNoiseHeight = float.MinValue;
        float halfWidth = mapWidth / 2;
        float halfHeight = mapWidth / 2;
        Perlin2D noiseGenerator = new Perlin2D(seed);
        System.Random rand = new System.Random(seed);

        float[,] noiseMap = new float[mapWidth, mapHeight];
        Vector2[] octaveOffsets = new Vector2[octaves];

        float amplitude = 1;

        float maxPossibleHeight = 0;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rand.Next(-100000, 100000) + offset.x;
            float offsetY = rand.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float noiseHeight = 0;
                float frequency = 1;
                amplitude = 1;
                for (int i = 0; i < octaves; i++)
                {
                    sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;
                    noisePoint = noiseGenerator.GetPerlinPoint(sampleX, sampleY);
                    noiseHeight += noisePoint * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;

            }
        }
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (normaliseMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalisedHeight = (noiseMap[x, y]) / (maxPossibleHeight * 1.2f);
                    noiseMap[x, y] = normalisedHeight;
                }
            }
        }
        return noiseMap;
    }
    public class Perlin2D
    {
        private static int[] p;

        public Perlin2D(int seed)
        {
            p = new int[512];
            System.Random rand = new System.Random(seed);
            for (int x = 0; x < 512; x++)
            {
                p[x] = rand.Next(1, 256);
            }
        }
        public float GetPerlinPoint(float x, float y)
        {
            x = (float)Math.Abs(x);
            y = (float)Math.Abs(y);
            int xi = (int)x & 255;
            int yi = (int)y & 255;
            float xf = x - (int)x;
            float yf = y - (int)y;
            float u = Fade(xf);
            float v = Fade(yf);

            int aa, ab, ba, bb;
            aa = p[p[xi] + yi];
            ab = p[p[xi] + inc(yi)];
            ba = p[p[inc(xi)] + yi];
            bb = p[p[inc(xi)] + inc(yi)];

            float x1, x2, y1;
            x1 = Lerp(Gradient(aa, xf, yf),
                        Gradient(ba, xf - 1, yf),
                        u);
            x2 = Lerp(Gradient(ab, xf, yf - 1),
                        Gradient(bb, xf - 1, yf - 1),
                          u);
            y1 = Lerp(x1, x2, v);

            return (y1 + 1) / 2f;
        }

        public int inc(int num)
        {
            num++;
            return num;
        }

        public static float Gradient(int hash, float x, float y)
        {
            switch (hash & 3)
            {
                case 0x0: return x + y;
                case 0x1: return -x + y;
                case 0x2: return x - y;
                case 0x3: return -x - y;
                default: return 0;
            }
        }

        public static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        public static float Lerp(float a, float b, float x)
        {
            return a + x * (b - a);
        }
    }
}





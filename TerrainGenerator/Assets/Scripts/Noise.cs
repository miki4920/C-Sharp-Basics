using UnityEngine;
using System;
using System.Runtime.InteropServices;

public static class Noise
{   public enum NormaliseMode { Local, Global};
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, int seed, Vector2 offset, NormaliseMode normaliseMode)
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

        float maxPossibleHeight= 0;

        for (int i=0; i<octaves;i++)
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
                    sampleX = (x-halfWidth + octaveOffsets[i].x)  / scale * frequency;
                    sampleY = (y-halfHeight + octaveOffsets[i].y) / scale * frequency;
                    noisePoint = noiseGenerator.GetPerlinPoint(sampleX, sampleY) * 2 - 1;
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
                if(normaliseMode == NormaliseMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalisedHeight = (noiseMap[x, y]+0.78f) / (maxPossibleHeight * 1.08f);
                    noiseMap[x, y] = normalisedHeight;
                }
            }
        }
        return noiseMap;
    }
    public class Perlin2D
    {
        private static Dictionary<Vector2, Vector2> vectors;
        private static GaussianRandom gaussianGenerator;
        public Perlin2D(int seed)
        {
            vectors = new Dictionary<Vector2, Vector2>();
            gaussianGenerator = new GaussianRandom(seed);
        }
        public float GetPerlinPoint(float x, float y)
        {
            x = (float)Math.Abs(x);
            y = (float)Math.Abs(y);
            int x0 = (int)x;
            int x1 = x0 + 1;
            int y0 = (int)y;
            int y1 = y0 + 1;

            float interpolationX = x - x0;
            float interpolationY = y - y0;

            float n0, n1, ix0, ix1, value;

            n0 = DotProduct(new Vector2(x0, y0), new Vector2(x, y));
            n1 = DotProduct(new Vector2(x1, y0), new Vector2(x, y));
            ix0 = Lerp(n0, n1, interpolationX);

            n0 = DotProduct(new Vector2(x0, y1), new Vector2(x, y));
            n1 = DotProduct(new Vector2(x1, y1), new Vector2(x, y));
            ix1 = Lerp(n0, n1, interpolationX);

            value = Lerp(ix0, ix1, interpolationY);

            return (value+2)/4;

        }

        public static Vector2 Gradient(Vector2 key)
        {
            Vector2 value = new Vector2();
            if(vectors.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                value = GetUnitVector();
                vectors.Add(key, value);
                return value;
            }
        }

        public static Vector2 GetUnitVector()
        {
            float x = gaussianGenerator.NextFloat(0, 1);
            float y = gaussianGenerator.NextFloat(0, 1);
            float magnitude = (float) Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            return new Vector2(x / magnitude, y / magnitude);
        }

        public static float DotProduct(Vector2 u, Vector2 v)
        {
            Vector2 vector = Gradient(u);
            float dx = Fade(1 - Math.Abs(v.X - u.X));
            float dy = Fade(1 - Math.Abs(v.Y - u.Y));
            return dx * vector.X + dy * vector.Y;
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






}





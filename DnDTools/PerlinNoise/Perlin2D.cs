
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace PerlinNoise
{
    public class Perlin2D
    {
        public float OctavePerlin(float x, float y, int octaves, float persistence, float lacunarity)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;
            for (int i = 0; i < octaves; i++)
            {
                total += GetPerlinPoint(x * frequency, y * frequency) * amplitude;

                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }   
            return total / maxValue;
        }
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







using System;
using System.Numerics;

namespace PerlinNoise
{
    public class Perlin2D
    {
        public float OctavePerlin(float x, float y, int octaves, float persistence)
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
                frequency *= 2;
            }   
            return total / maxValue;
        }
        private static int[] p;

        public Perlin2D(int seed)
        {
            p = new int[1024];
            System.Random rand = new System.Random(seed);
            for (int x = 0; x < 1024; x++)
            {
                p[x] = rand.Next(1, 512);
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






using System;
using System.Drawing;
using System.Linq;

namespace PerlinNoise
{
    class NoiseGenerator
    {
        static void Main(string[] args)
        {
            PerlinNoise.Perlin2D noiseGenerator = new PerlinNoise.Perlin2D(150);
            int size = 1024;
            int noisePoint = 0;
            float xInput;
            float yInput;
            Bitmap noise = new Bitmap(size, size);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    xInput = x * 1.0f / size;
                    yInput = y * 1.0f / size;
                    noisePoint = (int)(noiseGenerator.OctavePerlin(xInput, yInput, 2, 1.5f) * 256);
                    noise.SetPixel(x, y, Color.FromArgb(noisePoint, noisePoint, noisePoint));
                }
            }
            noise.Save("noise.bmp");
        }
    }
}

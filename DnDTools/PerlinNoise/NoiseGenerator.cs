using System.Drawing;

namespace PerlinNoise
{
    class NoiseGenerator
    {
        static void Main(string[] args)
        {
            PerlinNoise.Perlin2D noiseGenerator = new PerlinNoise.Perlin2D();
            int noisePoint = 0;
            int size = 1024;
            double xInput;
            double yInput;
            Bitmap noise = new Bitmap(size, size);
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    xInput = x * 1.0 / size;
                    yInput = y * 1.0 / size;
                    noisePoint = (int)(noiseGenerator.OctavePerlin(xInput, yInput, 6, 1.5) * 256);
                    noise.SetPixel(x, y, Color.FromArgb(noisePoint, noisePoint, noisePoint));
                }
            }
            noise.Save("noise.bmp");
        }
    }
}

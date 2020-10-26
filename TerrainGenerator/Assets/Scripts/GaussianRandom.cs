using System;


class GaussianRandom
{
    readonly Random randomGenerator;
    public GaussianRandom(int seed)
    {
        randomGenerator = new Random(seed);
    }
    double GenerateNumber()
    {
        double u1 = 1 - randomGenerator.NextDouble();
        double u2 = 1 - randomGenerator.NextDouble();
        double randomStandardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
            Math.Cos(2.0 * Math.PI * u2);
        return randomStandardNormal;
    }
    public float NextFloat(int mean, int std)
    {
        return (float)(mean + std * GenerateNumber());

    }

    public double NextDouble(int mean, int std)
    {
        return (mean + std * GenerateNumber());
    }
}


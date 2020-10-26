using System;
using System.Linq;

namespace BusinessRolls
{
    class BusinessHelper
    {
        static void Main(string[] args)
        {
            BusinessCalculator calculator = new BusinessCalculator();
            int value;
            int bonus;
            int numberOfRolls;
            Console.WriteLine((int)-0.5);
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter value of your business: ");
                    value = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter your bonus: ");
                    bonus = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter number of rolls: ");
                    numberOfRolls = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Your total profit: {0}", calculator.CalculateBusiness(value, bonus, numberOfRolls));
                }
                catch (System.FormatException) {
                    Console.WriteLine("Sorry, we only accept numbers. You will need to try again.");
                }
            }
        }
    }

    class BusinessCalculator
    {
        public double CalculateBusiness(int value, int bonus, int numberOfRolls)
        {
            var randomiser = new Random();
            double[] allValues = new double[numberOfRolls];
            int d100;
            int d20;
            int total;
            double businessValue = value * 0.05;

            for (int i = 0; i < numberOfRolls; i++)
            {
                d100 = randomiser.Next(1, 100);
                d20 = randomiser.Next(1, 20);
                total = d100 + d20 + bonus;
                if (d20 == 20)
                {
                    total += d20 + bonus;
                }
                else if (d20 == 1)
                {
                    total -= 2 * (d20 + bonus);
                }
                if (-100 <= total && total <= 20)
                {
                    allValues[i] = businessValue * -1.5;
                }
                else if (21 <= total && total <= 30)
                {
                    allValues[i] = businessValue * -1.0;
                }
                else if (31 <= total && total <= 40)
                {
                    allValues[i] = businessValue * -0.5;
                }
                else if (41 <= total && total <= 60)
                {
                    allValues[i] = businessValue * 0.0;
                }
                else if (61 <= total && total <= 80)
                {
                    allValues[i] = businessValue * 1.0;
                }
                else if (81 <= total && total <= 90)
                {
                    allValues[i] = businessValue * 2.0;
                }
                else if (91 <= total)
                {
                    allValues[i] = businessValue * 3.0;
                }
            }
            return allValues.Sum();
        }
    }
}

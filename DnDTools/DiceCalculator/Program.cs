using System;


namespace DungeonProgram
{ 
    class Program
    {
        static void Main()
        {   while (true)
            {
                DiceAverageCalculator calculator = new DiceAverageCalculator();
                Console.WriteLine("Please Insert dice equation: ");
                string[] userInput = Console.ReadLine().ToLower().Replace(" ", "").Split("+");
                Console.WriteLine("The average equals to {0}\n", calculator.CalculateAverage(userInput));
            }
        }
    }
    class DiceAverageCalculator
    {
        double CalculateDieAverage(string die)
        {
            string[] dieElements = die.Split("d");
            int dieCount = Convert.ToInt32(dieElements[0]);
            int dieValue = Convert.ToInt32(dieElements[1]);
            return dieCount * ((1 + dieValue) / 2.0);
            
        }
        public double CalculateAverage(string[] diceInput) {
            double totalValue = 0;
            int converter;
            foreach (string die in diceInput)
            {
                bool canConvert = int.TryParse(die, out converter);
                if (canConvert)
                {
                    totalValue += Convert.ToInt32(die);
                }
                else
                {
                    totalValue += CalculateDieAverage(die);
                }
            }
            return totalValue;
        }
    }
}
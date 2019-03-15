using System;

namespace AsigurityLightweight.Utilities
{
    public static class LevenshteinDistance
    {
        private static int ComputeDistance(string FirstString, string SecondString)
        {
            int FirstStringSize = FirstString.Length;
            int SecondStringSize = SecondString.Length;
            int CostResult = 0;
            int[][] PatternMatrix = new int[FirstStringSize + 1][];

            for (int i = 0; i <= FirstStringSize; i++)
            {
                PatternMatrix[i] = new int[SecondStringSize + 1];
            }
            if (FirstStringSize == 0)
                return SecondStringSize;
            if (SecondStringSize == 0)
                return FirstStringSize;
            for (int i = 0; i <= FirstStringSize; PatternMatrix[i][0] = i++)
            {
                /* Avoidable */
            }
            for (int j = 0; j <= SecondStringSize; PatternMatrix[0][j] = j++)
            {
                /* Avoidable */
            }
            for (int i = 1; i <= FirstStringSize; i++)
            {
                for (int j = 1; j <= SecondStringSize; j++)
                {
                    CostResult = (SecondString[j - 1] == FirstString[i - 1]) ? 0 : 1;
                    PatternMatrix[i][j] = Math.Min(Math.Min(PatternMatrix[i - 1][j] + 1, PatternMatrix[i][j - 1] + 1), PatternMatrix[i - 1][j - 1] + CostResult);
                }
            }
            return PatternMatrix[FirstStringSize][SecondStringSize];
        }

        public static double GetLevenshteinPercentage(string FirstString, string SecondString)
        {
            return (double)ComputeDistance(FirstString, SecondString) / Math.Max(FirstString.Length, SecondString.Length);
        }
    }
}
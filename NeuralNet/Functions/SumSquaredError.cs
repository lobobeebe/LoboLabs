using System;
using System.Collections.Generic;

namespace LoboLabs
{
namespace NeuralNet
{
namespace Functions
{

public class SumSquaredError : ErrorFunction
{
    /// <summary>
    /// Constructor
    /// TODO: Unit Tests
    /// </summary>
    public SumSquaredError()
    {
    }
        
    /// <summary>
    /// Calculates the error between the expected and actual values by summing the squares of the difference between each index.
    /// If one vector is longer than the other, each value in the longer vector is squared and added to the error.
    /// </summary>
    /// <param name="expectedValues">Expected values</param>
    /// <param name="actualValues">Actual values</param>
    /// <returns>The Sum Squared Error between the expected and actual valuess</returns>
    public override double Error(List<double> expectedValues, List<double> actualValues)
    {
        double error = 0.0;
        double sumSquaredError = 0.0;

        int numValues = Math.Max(expectedValues.Count, actualValues.Count);
        for (int i = 0; i < numValues; ++i)
        {
            if(i < expectedValues.Count && i < actualValues.Count)
            {
                error = Math.Abs(expectedValues[i] - actualValues[i]);
            }
            else
            {
                if (i < expectedValues.Count)
                {
                    error = expectedValues[i];
                }
                else
                {
                    error = actualValues[i];
                }
            }

            sumSquaredError += error * error;
        }

        return .5 * sumSquaredError;
    }
}

}
}
}

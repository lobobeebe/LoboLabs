using System.Collections.Generic;

namespace LoboLabs
{
namespace NeuralNet
{
namespace Functions
{

/// <summary>
/// Represents a generic function for calculating error between vectors
/// </summary>
public abstract class ErrorFunction
{
    /// <summary>
    /// Finds the error between the expected values and actual values.
    /// </summary>
    /// <param name="expectedValues">The expected values</param>
    /// <param name="actualValues">The actual values</param>
    /// <returns>The error between the two sets of values</returns>
    public abstract double Error(List<double> expectedValues, List<double> actualValues);
}

}
}
}
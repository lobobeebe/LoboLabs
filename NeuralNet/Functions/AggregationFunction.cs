using System.Collections.Generic;

namespace LoboLabs
{
namespace NeuralNet
{
namespace Functions
{

/// <summary>
/// Represents a generic InterpretationFunction
/// TODO: Come up with a different name for this
/// </summary>
public abstract class AggregationFunction
{
    /// <summary>
    /// Interprets the input values as another set of values.
    /// </summary>
    /// <param name="input">The input to be interpreted</param>
    /// <returns>The interpreted data</returns>
    public abstract List<double> Apply(List<double> input);

    /// <summary>
    /// Reverses the interpretation made to the inputs.
    /// </summary>
    /// <param name="input">The input to reverse interpret</param>
    /// <returns>The pre-interpreted data</returns>
    public abstract double ApplyDerivative(double input);
}

}
}
}
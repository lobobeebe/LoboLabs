using System;
using System.Collections.Generic;

namespace LoboLabs
{
namespace NeuralNet
{
namespace Functions
{

/// <summary>
/// TODO: Documentation
/// TODO: Unit Tests
/// </summary>
public class SoftmaxFunction : AggregationFunction
{
    /// <summary>
    /// TODO: Documentation
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public override List<double> Apply(List<double> input)
    {
        double sum = 0.0;
        List<double> result = new List<double>(input.Count);

        for (int i = 0; i < input.Count; ++i)
        {
            result.Add(EstimateExp(input[i]));
            sum += result[i];
        }

        // Normalize values
        if (sum != 0)
        {
            for (int i = 0; i < input.Count; ++i)
            {
                result[i] /= sum;
            }
        }

        return result;
    }

    /// <summary>
    /// TODO: Documentation
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override double ApplyDerivative(double value)
    {
	    return (1 - value) * value;
    }

    /// <summary>
    /// TODO: Documentation
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private double EstimateExp(double input)
    {
	    double estimate;
            
		estimate = Math.Exp(input);

	    return estimate;
    }
}

}
}
}
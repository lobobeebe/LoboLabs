using System;

namespace LoboLabs
{
namespace NeuralNet
{
namespace Functions
{

/// <summary>
/// Tanh Activation Function 
/// TODO: Unit Tests
/// </summary>
public class HyperbolicTangent : ActivationFunction
{
    /// <summary>
    /// Constructor
    /// </summary>
    public HyperbolicTangent()
    {
    }

    /// <summary>
    /// See AcitvationFunction.Apply(double input)
    /// </summary>
    public override double Apply(double input)
    {
        // This approximation is correct to 30 decimals.
        if (input < -20.0)
        {
            return -1.0;
        }
        else if (input > 20.0)
        {
            return 1.0;
        }
        else
        {
            return Math.Tanh(input);
        }
    }

    /// <summary>
    /// See ActivationFunction.ApplyDerivative(double input)
    /// </summary>
    public override double ApplyDerivative(double input)
    {
        return (1 + input) * (1 - input);
    }
}

}
}
}
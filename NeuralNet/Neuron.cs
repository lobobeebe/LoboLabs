namespace LoboLabs
{ 
namespace NeuralNet
{

/// <summary>
/// Represents a hidden node in a NeuralNet
/// TODO: Unit Test
/// </summary>
public class Neuron : ComputationalNode
{
    public Neuron(Functions.ActivationFunction activationFunction)
    { 
        // Activation
        ActivationFunction = activationFunction;
    }

    /// <summary>
    /// Activation Function to apply when computing
    /// </summary>
    private Functions.ActivationFunction ActivationFunction
    {
        get;
        set;
    }

    protected override double PostProcessComputation(double computation)
    {
        // Activation Function
        return ActivationFunction.Apply(computation);
    }

    protected override double PostProcessErrorSignalAggregation(double errorSignalAggregation)
    {
        // Multiply by the derivative of the activation function for aggregated error signal
        return errorSignalAggregation * ActivationFunction.ApplyDerivative(LastOutput);
    }
}

}
}

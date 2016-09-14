namespace LoboLabs
{

using Utilities;

namespace NeuralNet
{

/// <summary>
/// Represents a generic NeuralNetworkGenerator
/// </summary>
public abstract class NeuralNetworkGenerator
{
    private static double DEFAULT_MAX_WEIGHT = 0.001;
    private static double DEFAULT_MIN_WEIGHT = 0.0001;

    /// <summary>
    /// Constructor
    /// </summary>
    public NeuralNetworkGenerator()
    {
        MaxWeight = DEFAULT_MAX_WEIGHT;
        MinWeight = DEFAULT_MIN_WEIGHT;
    }

    public abstract NeuralNetwork Generate();

    protected double GetNextWeight()
    {
        return MathUtils.NextRand(MinWeight, MaxWeight);
    }

    public double MaxWeight
    {
        get;
        set;
    }

    public double MinWeight
    {
        get;
        set;
    }
}

}
}
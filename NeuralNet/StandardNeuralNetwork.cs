namespace LoboLabs
{
namespace NeuralNet
{

using Functions;

public class StandardNeuralNetwork : NeuralNetwork
{
    public StandardNeuralNetwork(AggregationFunction aggregationFunction,
        ErrorFunction errorFunction) : base(aggregationFunction, errorFunction)
    {
    }
}

}
}
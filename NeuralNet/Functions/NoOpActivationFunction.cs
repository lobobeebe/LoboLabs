namespace LoboLabs
{
namespace NeuralNet
{
namespace Functions
{

public class NoOpActivationFunction : ActivationFunction
{
    public override double Apply(double input)
    {
        return input;
    }

    public override double ApplyDerivative(double input)
    {
        return 1;
    }
}

}
}
}

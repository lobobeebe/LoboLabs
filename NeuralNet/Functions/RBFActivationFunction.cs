namespace LoboLabs.NeuralNet.Functions
{

    /// <summary>
    /// Represents a Radial Basis Function
    /// </summary>
    public class RBFActivationFunction : ActivationFunction
    {
        public override double Apply(double input)
        {
            return System.Math.Exp(-(input * input));
        }

        public override double ApplyDerivative(double input)
        {
            return -2 * input * Apply(input);
        }
    }

}
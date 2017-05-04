using System;

namespace LoboLabs.NeuralNet.Functions
{

    /// <summary>
    /// Represents a Radial Basis Function
    /// </summary>
    public class RBFActivationFunction : ActivationFunction
    {
        public static string FUNCTION_NAME = "RBF";

        public override double Apply(double input)
        {
            return Math.Exp(-(input * input));
        }

        public override double ApplyDerivative(double input)
        {
            return -2 * input * Apply(input);
        }

        public override string GetFunctionName()
        {
            return FUNCTION_NAME;
        }
    }

}
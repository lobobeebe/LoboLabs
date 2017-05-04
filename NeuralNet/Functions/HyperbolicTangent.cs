using System;

namespace LoboLabs.NeuralNet.Functions
{

    /// <summary>
    /// Tanh Activation Function 
    /// TODO: Unit Tests
    /// </summary>
    public class HyperbolicTangent : ActivationFunction
    {
        public static string FUNCTION_NAME = "HYPERBOLIC";

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
            double output;

            // This approximation is correct to 30 decimals.
            if (input < -20.0)
            {
                output = -1.0;
            }
            else if (input > 20.0)
            {
                output = 1.0;
            }
            else
            {
                output = Math.Tanh(input);
            }

            return output;
        }

        /// <summary>
        /// See ActivationFunction.ApplyDerivative(double input)
        /// </summary>
        public override double ApplyDerivative(double input)
        {
            //return 1 - Math.Pow(Apply(input), 2);
            return (1 + input) * (1 - input);
        }

        public override string GetFunctionName()
        {
            return FUNCTION_NAME;
        }
    }
}
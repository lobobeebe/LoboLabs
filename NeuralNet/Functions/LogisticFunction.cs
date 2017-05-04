using System;

namespace LoboLabs.NeuralNet.Functions
{

    /// <summary>
    /// Logistic Activation Function 
    /// TODO: Unit Tests
    /// </summary>
    public class LogisticFunction : ActivationFunction
    {
        public static string FUNCTION_NAME = "LOGISTIC";

        /// <summary>
        /// Constructor
        /// </summary>
        public LogisticFunction()
        {
        }

        /// <summary>
        /// See AcitvationFunction.Apply(double input)
        /// </summary>
        public override double Apply(double input)
        {
            return 1 / (1 + Math.Exp(-input));
        }

        /// <summary>
        /// See ActivationFunction.ApplyDerivative(double input)
        /// </summary>
        public override double ApplyDerivative(double input)
        {
            return Apply(input) * (1 - Apply(input));
        }

        public override string GetFunctionName()
        {
            return FUNCTION_NAME;
        }
    }

}
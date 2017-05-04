namespace LoboLabs.NeuralNet.Functions
{

    /// <summary>
    /// Represents a generic Activation Function
    /// </summary>
    public abstract class ActivationFunction
    {
        /// <summary>
        /// Applies the defined function on the input 
        /// </summary>
        /// <param name="input">Input on which the function will be applied</param>
        /// <returns>The result of the activation function</returns>
        public abstract double Apply(double input);

        /// <summary>
        /// Applies the derivative of the defined function on the input
        /// </summary>
        /// <param name="input">Input on which the function's derivative will be applied</param>
        /// <returns>The result of the derivative of the activation function</returns>
        public abstract double ApplyDerivative(double input);

        public bool Equals(ActivationFunction other)
        {
            return GetFunctionName().Equals(other.GetFunctionName());
        }

        public abstract string GetFunctionName();
    }

}
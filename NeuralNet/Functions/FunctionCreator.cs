namespace LoboLabs.NeuralNet.Functions
{
    public class FunctionCreator
    {
        public static ActivationFunction CreateActivationFunction(string functionName)
        {
            ActivationFunction function = null;
            
            if (functionName == HyperbolicTangent.FUNCTION_NAME)
            {
                function = new HyperbolicTangent();
            }
            else if (functionName == LogisticFunction.FUNCTION_NAME)
            {
                function = new LogisticFunction();
            }
            else if (functionName == NoOpActivationFunction.FUNCTION_NAME)
            {
                function = new NoOpActivationFunction();
            }
            else if (functionName == RBFActivationFunction.FUNCTION_NAME)
            {
                function = new RBFActivationFunction();
            }

            return function;
        }
    }
}

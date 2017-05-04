namespace LoboLabs.NeuralNet.Functions
{

    public class NoOpActivationFunction : ActivationFunction
    {
        public static string FUNCTION_NAME = "NO_OP";

        public override double Apply(double input)
        {
            return input;
        }

        public override double ApplyDerivative(double input)
        {
            return 1;
        }

        public override string GetFunctionName()
        {
            return FUNCTION_NAME;
        }
    }
}
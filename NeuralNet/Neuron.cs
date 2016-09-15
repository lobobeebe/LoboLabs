namespace LoboLabs.NeuralNet
{

    /// <summary>
    /// Represents a hidden node in a NeuralNet
    /// TODO: Unit Test
    /// </summary>
    public class Neuron : ComputationalNode
    {
        public Neuron(Functions.ActivationFunction activationFunction) :
            base(activationFunction)
        { 
        }
    }

}

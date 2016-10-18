using System.Collections.Generic;

namespace LoboLabs.NeuralNet.Mock
{

    public class ComputationalNode : NeuralNet.ComputationalNode
    {
        public ComputationalNode(Functions.ActivationFunction activationFunction) :
            base(activationFunction)
        {
        }
    }

}

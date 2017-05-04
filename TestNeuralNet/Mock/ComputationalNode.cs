using System.Collections.Generic;
using System.IO;

namespace LoboLabs.NeuralNet.Mock
{

    public class ComputationalNode : NeuralNet.ComputationalNode
    {
        public ComputationalNode(BinaryReader reader) : base(reader)
        {
        }

        public ComputationalNode(Functions.ActivationFunction activationFunction) :
            base(activationFunction)
        {
        }
    }

}

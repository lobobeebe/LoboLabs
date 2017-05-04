using System.Collections.Generic;
using System.IO;

namespace LoboLabs.NeuralNet.Mock
{

    public class ComputationalNode : NeuralNet.ComputationalNode
    {
        public ComputationalNode(NeuralNetwork parent, BinaryReader reader) : base(parent, reader)
        {
        }

        public ComputationalNode(NeuralNetwork parent, Functions.ActivationFunction activationFunction) :
            base(parent, activationFunction)
        {
        }
    }

}

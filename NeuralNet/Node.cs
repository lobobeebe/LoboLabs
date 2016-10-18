using System.Collections.Generic;

using LoboLabs.NeuralNet.Messaging;
using LoboLabs.Utilities;

namespace LoboLabs.NeuralNet
{
    public class Node
    {
        public Node(bool isSumming = false)
        {
            IsSumming = isSumming;
        }

        public void AddErrorSignal(double errorSignal)
        {
            if (IsSumming)
            {
                ErrorSignalSum += errorSignal;
            }
        }

        private double ErrorSignalSum
        {
            get;
            set;
        }

        public double GetAndResetErrorSignalSum()
        {
            double errorSignalSum = ErrorSignalSum;
            ErrorSignalSum = 0;

            return errorSignalSum;
        }

        private bool IsSumming
        {
            get;
            set;
        }

        /// <summary>
        /// Stores the last output of this Neuron
        /// </summary>
        public double LastOutput
        {
            get;
            set;
        }
    }

}

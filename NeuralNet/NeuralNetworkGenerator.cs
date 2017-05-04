using LoboLabs.NeuralNet.Functions;
using LoboLabs.Utilities;
using System.Collections.Generic;

namespace LoboLabs.NeuralNet
{
    /// <summary>
    /// Represents a generic NeuralNetworkGenerator
    /// </summary>
    public class NeuralNetworkCreator
    {
        public static int DEFAULT_NUM_LAYERS = 3;
        public static int DEFAULT_NUM_HIDDEN_NEURONS = 3;

        private static double DEFAULT_MAX_WEIGHT = 0.001;
        private static double DEFAULT_MIN_WEIGHT = 0.0001;

        /// <summary>
        /// Constructor
        /// </summary>
        public NeuralNetworkCreator()
        {
            MaxWeight = DEFAULT_MAX_WEIGHT;
            MinWeight = DEFAULT_MIN_WEIGHT;
            NumHidden = DEFAULT_NUM_HIDDEN_NEURONS;
        }

        public NeuralNetwork Create(uint numInputs, int numOutputs)
        {
            NeuralNetwork network = new NeuralNetwork();

            List<Node> sensors = new List<Node>();
            for (int sensor = 0; sensor < numInputs; ++sensor)
            {
                sensors.Add(new Node());
            }

            List<List<ComputationalNode>> neurons = new List<List<ComputationalNode>>();

            // Add Hidden Layer
            neurons.Add(new List<ComputationalNode>());

            // Add Actuator Layer and Actuator
            neurons.Add(new List<ComputationalNode>());
            for (int actuatorIndex = 0; actuatorIndex < numOutputs; ++actuatorIndex)
            {
                neurons[1].Add(new ComputationalNode(new LogisticFunction()));
                neurons[1][actuatorIndex].Bias = GetNextWeight();
            }

            // Add a number of hidden neurons
            for (int neuron = 0; neuron < NumHidden; ++neuron)
            {
                // Add the new Node
                neurons[0].Add(new ComputationalNode(new LogisticFunction()));

                // Add all sensors as inputs - TODO: This only works with two layers
                foreach (Node sensor in sensors)
                {
                    neurons[0][neuron].RegisterInput(sensor, GetNextWeight());
                    neurons[0][neuron].Bias = GetNextWeight();
                }

                // Add each node to the actuator's inputs
                // TODO: This only works with two layers and 1 actuator
                for (int actuatorIndex = 0; actuatorIndex < numOutputs; ++actuatorIndex)
                {
                    neurons[1][actuatorIndex].RegisterInput(neurons[0][neuron], GetNextWeight());
                }
            }

            // Assign lists to the network
            network.Sensors = sensors;
            network.Neurons = neurons;

            return network;
        }

        private double GetNextWeight()
        {
            return MathUtils.NextRand(MinWeight, MaxWeight);
        }

        public double MaxWeight
        {
            get;
            set;
        }

        public double MinWeight
        {
            get;
            set;
        }

        public int NumHidden
        {
            get;
            set;
        }
    }
}
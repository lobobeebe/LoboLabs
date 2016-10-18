using System.Collections.Generic;
using LoboLabs.NeuralNet;
using LoboLabs.NeuralNet.Functions;

namespace LoboLabs.GestureNeuralNet
{
    public class BinaryGestureNeuralNetworkGenerator : NeuralNetworkGenerator
    {
        private static int DEFAULT_NUM_NEURONS = 1;
        private static int DEFAULT_NUM_LAYERS = 1;
        private static int DEFAULT_NUM_SENSORS = 3;
        private static int DEFAULT_NUM_ACTUATORS = 1;

        public BinaryGestureNeuralNetworkGenerator()
        {
            NumLayers = DEFAULT_NUM_LAYERS;
            NumHidden = DEFAULT_NUM_NEURONS;
            NumInputs = DEFAULT_NUM_SENSORS;
            NumOutputs = DEFAULT_NUM_ACTUATORS;
        }

        public override NeuralNetwork Generate()
        {
            BinaryGestureNeuralNetwork network = new BinaryGestureNeuralNetwork();

            // Because vector positions are the inputs to Gestures, there will always be 3 inputs.
            List<Node> sensors = new List<Node>(NumInputs);
            for (int sensor = 0; sensor < NumInputs; ++sensor)
            {
                sensors.Add(new Node());
            }

            // Two layers of neurons because one hidden layer and one layer of actuators
            List<List<ComputationalNode>> neurons = new List<List<ComputationalNode>>(2);

            // Add two Hidden Layers
            neurons.Add(new List<ComputationalNode>());

            // Add Actuator Layer and Actuator
            neurons.Add(new List<ComputationalNode>());
            neurons[1].Add(new ComputationalNode(new LogisticFunction()));
            neurons[1][0].Bias = GetNextWeight();

            // Add a number of hidden neurons
            for (int layer = 0; layer < NumLayers; ++layer)
            {
                for (int neuron = 0; neuron < NumHidden; ++neuron)
                {
                    // Add the new Node
                    neurons[layer].Add(new ComputationalNode(new LogisticFunction()));

                    // Recurrent link to self
                    //neurons[layer][neuron].RegisterInput(neurons[0][neuron], GetNextWeight());

                    // Add all sensors as inputs - TODO: This only works with two layers
                    foreach (Node sensor in sensors)
                    {
                        neurons[layer][neuron].RegisterInput(sensor, GetNextWeight());
                        neurons[layer][neuron].Bias = GetNextWeight();
                    }

                    // Add each node to the actuator's inputs
                    // TODO: This only works with two layers and 1 actuator
                    neurons[1][0].RegisterInput(neurons[layer][neuron], GetNextWeight());
                }
            }

            // Assign lists to the network
            network.Sensors = sensors;
            network.Neurons = neurons;

            return network;
        }

        public int NumHidden
        {
            get;
            set;
        }

        private int NumInputs
        {
            get;
            set;
        }

        private int NumLayers
        {
            get;
            set;
        }

        private int NumOutputs
        {
            get;
            set;
        }
    }

}

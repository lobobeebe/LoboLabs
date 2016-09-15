using System.Collections.Generic;
using LoboLabs.NeuralNet;
using LoboLabs.NeuralNet.Functions;

namespace LoboLabs.GestureNeuralNet
{
    public class BinaryGestureNeuralNetworkGenerator : NeuralNetworkGenerator
    {
        private static int DEFAULT_NUM_NEURONS = 1;
        private static int DEFAULT_NUM_SENSORS = 3;
        private static int DEFAULT_NUM_ACTUATORS = 1;

        public BinaryGestureNeuralNetworkGenerator()
        {
            NumHidden = DEFAULT_NUM_NEURONS;
            NumInputs = DEFAULT_NUM_SENSORS;
            NumOutputs = DEFAULT_NUM_ACTUATORS;
        }

        public override NeuralNetwork Generate()
        {
            BinaryGestureNeuralNetwork network = new BinaryGestureNeuralNetwork();

            // Because vector positions are the inputs to Gestures, there will always be 3 inputs.
            List<Sensor> sensors = new List<Sensor>(NumInputs);
            for (int sensor = 0; sensor < NumInputs; ++sensor)
            {
                sensors.Add(new Sensor());
            }

            // Currently, gesture detection is binary - True or False. Always 1 Actuator.
            List<Actuator> actuators = new List<Actuator>(NumOutputs);
            for (int actuator = 0; actuator < NumOutputs; ++actuator)
            {
                actuators.Add(new Actuator());
            }

            // Add a number of hidden neurons
            List<Neuron> neurons = new List<Neuron>(NumHidden);
            for (int neuron = 0; neuron < NumHidden; ++neuron)
            {
                neurons.Add(new Neuron(new HyperbolicTangent()));
                
                foreach (Sensor sensor in sensors)
                {
                    neurons[neuron].RegisterInput(sensor, GetNextWeight());
                    sensor.RegisterOutput(neurons[neuron]);
                }

                foreach (Actuator actuator in actuators)
                {
                    neurons[neuron].RegisterOutput(actuator);
                    actuator.RegisterInput(neurons[neuron], GetNextWeight());
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

        private int NumOutputs
        {
            get;
            set;
        }
    }

}

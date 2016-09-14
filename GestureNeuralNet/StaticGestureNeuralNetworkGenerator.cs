using System.Collections.Generic;

namespace LoboLabs
{

using NeuralNet;
using NeuralNet.Functions;

namespace GestureNeuralNet
{

public class StaticGestureNeuralNetworkGenerator : NeuralNetworkGenerator
{
    private static int DEFAULT_NUM_NEURONS = 3;
    private static int DEFAULT_NUM_SENSORS = 30;
    private static int DEFAULT_NUM_ACTUATORS = 3;

    public StaticGestureNeuralNetworkGenerator()
    {
        NumHidden = DEFAULT_NUM_NEURONS;
        NumInputs = DEFAULT_NUM_SENSORS;
        NumOutputs = DEFAULT_NUM_ACTUATORS;
    }

    public override NeuralNetwork Generate()
    {
        StaticGestureNeuralNetwork network = new StaticGestureNeuralNetwork();

        List<Sensor> sensors = new List<Sensor>(NumInputs);
        for (int sensor = 0; sensor < NumInputs; ++sensor)
        {
            sensors.Add(new Sensor());
        }

        List<Actuator> actuators = new List<Actuator>(NumOutputs);
        for (int actuator = 0; actuator < NumOutputs; ++actuator)
        {
            actuators.Add(new Actuator());
            actuators[actuator].Bias = GetNextWeight();
            actuators[actuator].RegisterOutput(network);
        }

        List<Neuron> neurons = new List<Neuron>(NumHidden);
        for (int neuron = 0; neuron < NumHidden; ++neuron)
        {
            neurons.Add(new Neuron(new HyperbolicTangent()));
            neurons[neuron].Bias = neuron;

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
        network.Actuators = actuators;

        return network;
    }

    public int NumHidden
    {
        get;
        set;
    }

    public int NumInputs
    {
        get;
        set;
    }

    public int NumOutputs
    {
        get;
        set;
    }
}

}
}

using System.Collections.Generic;

namespace LoboLabs
{
namespace NeuralNet
{

using Functions;

public class StandardNeuralNetworkGenerator : NeuralNetworkGenerator
{
    private static int DEFAULT_NUM_HIDDEN = 3;
    private static int DEFAULT_NUM_INPUTS = 4;
    private static int DEFAULT_NUM_OUTPUTS = 4;

    public StandardNeuralNetworkGenerator()
    {
        NumHidden = DEFAULT_NUM_HIDDEN;
        NumInputs = DEFAULT_NUM_INPUTS;
        NumOutputs = DEFAULT_NUM_OUTPUTS;
    }

    public override NeuralNetwork Generate()
    {
        StandardNeuralNetwork network = new StandardNeuralNetwork(new SoftmaxFunction(), new SumSquaredError());

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
            neurons.Add(new Neuron(new Functions.HyperbolicTangent()));
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

using System.Collections.Generic;

namespace LoboLabs
{

using Utilities;

namespace NeuralNet
{

using Messaging;

/// <summary>
/// Represents a generic NeuralNetwork
/// </summary>
public abstract class NeuralNetwork : MessageProcessor, ScapeListener
{
    private static ClassLogger Logger = new ClassLogger(typeof(NeuralNetwork));

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="aggregationFunction"></param>
    /// <param name="errorFunction"></param>
    public NeuralNetwork(Functions.AggregationFunction aggregationFunction, Functions.ErrorFunction errorFunction)
    {
        AggregationFunction = aggregationFunction;
        ErrorFunction = errorFunction;

        Sensors = new List<Sensor>();
        Neurons = new List<Neuron>();
        Actuators = new List<Actuator>();
        ActuatorInputs = new Dictionary<NeuralNode, InputData>();

        // Should match the order of Actuators
        OutputData = new List<double>();
    }    

    private Dictionary<NeuralNode, InputData> ActuatorInputs
    {
        get;
        set;
    }

    /// <summary>
    /// Vector of actuators that belong to this Neural Net
    /// </summary>
    public List<Actuator> Actuators
    {
        get;
        set;
    }

    /// <summary>
    /// Function to aggregate and interpret the final data
    /// </summary>
    private Functions.AggregationFunction AggregationFunction
    {
        get;
        set;
    }

    private double CalculateMeanError(List<KnownData> trainData)
    {
        double error = 0.0;
        double sumError = 0.0;

        for (int i = 0; i < trainData.Count; ++i)
        {
            List<double> expectedOutput = trainData[i].ExpectedOutputs;
            List<double> actualOutput = Compute(trainData[i].InputValues);

            error = ErrorFunction.Error(expectedOutput, actualOutput);

            sumError += error;
        }

        return (trainData.Count > 0) ? (sumError / trainData.Count) : 0;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    protected List<double> Compute(List<double> inputs)
    {
        // Send input information into Network via Sensors
        for (int i = 0; i < Sensors.Count; ++i)
        {
            SendMessage(Sensors[i], new DataMessage(this, inputs[i]));
        }

        return AggregationFunction.Apply(OutputData);
    }

    /// <summary>
    /// TODO
    /// </summary>
    private Functions.ErrorFunction ErrorFunction
    {
        get;
        set;
    }

    /// <summary>
    /// Vector of Neurons that belong to this Neural Net
    /// </summary>
    public List<Neuron> Neurons
    {
        get;
        set;
    }

    public void OnReceiveScapeData(List<double> data)
    {
        OnResult(Compute(data));
    }

    protected virtual void OnResult(List<double> result)
    {
        // Default Implementation
    }

    /// <summary>
    /// The Output in order of the list of Actuators
    /// </summary>
    private List<double> OutputData
    {
        get;
        set;
    }

    public override void ProcessMessage(Message message)
    {
        int index;
        bool isFound = VectorUtils.GetIndex(Actuators, (Actuator)message.Sender, out index);

        if (isFound)
        {
            if (message.MessageType == DataMessage.MESSAGE_TYPE)
            {
                while (OutputData.Count <= index)
                {
                    OutputData.Add(0);
                }

                OutputData[index] = (message as DataMessage).Value;
            }
        }
    }

    /// <summary>
    /// Vector of sensors that belong to this Neural Net
    /// </summary>
    public List<Sensor> Sensors
    {
        get;
        set;
    }

    public void TrainBackPropagation(List<KnownData> trainingData, int maxEpochs, double learningRate)
    {
	    // Interval to check error
	    int errInterval = maxEpochs / 10;
	
	    for (int epoch = 0; epoch < maxEpochs; ++epoch)
	    {
		    if (errInterval == 0 || epoch % errInterval == 0)
		    {
			    double meanError = CalculateMeanError(trainingData);
			    Logger.Debug("Epoch " + epoch + ":  Error = " +
				    meanError);
		    }
		
		    for (int index = 0; index < trainingData.Count; ++index)
		    {
			    List<double> inputs = trainingData[index].InputValues;
			    List<double> expectedOutputs = trainingData[index].ExpectedOutputs;

			    List<double> actualOutputs = Compute(inputs);

			    for (int actuator = 0; actuator < Actuators.Count; ++actuator)
                {
                    double outputDifference = expectedOutputs[actuator] - actualOutputs[actuator];

                    double errorSignal = outputDifference * AggregationFunction.ApplyDerivative(actualOutputs[actuator]);
                    SendMessage(Actuators[actuator], new ErrorSignalUpdateMessage(this, errorSignal));
			    }
		    }
	    }
    }
}

}
}

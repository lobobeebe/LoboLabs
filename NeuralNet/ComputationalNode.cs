using System.Collections.Generic;

namespace LoboLabs
{

using Utilities;

namespace NeuralNet
{

using Messaging;

public abstract class ComputationalNode : NeuralNode
{
    private const double LEARNING_RATE = 0.05;
    private static ClassLogger Logger = new ClassLogger(typeof(ComputationalNode));

    public ComputationalNode()
    {
        // Inputs
        Inputs = new List<MessageProcessor>();
        InputDataMap = new Dictionary<MessageProcessor, InputData>();

        OutputDataMap = new Dictionary<MessageProcessor, OutputData>();
    }
        
    private double AggregateErrorSignals()
    {
        double errorSignalAggregation = 0;

        foreach (MessageProcessor output in Outputs)
        {
            errorSignalAggregation += OutputDataMap[output].ErrorSignal;
        }

        return PostProcessErrorSignalAggregation(errorSignalAggregation);
    }

    private bool AreAllErrorSignalsValid()
    {
        foreach (MessageProcessor output in Outputs)
        {
            if (!OutputDataMap[output].IsValid)
            {
                return false;
            }
        }

        return true;
    }

    private bool AreAllInputsValid()
    {
        foreach (MessageProcessor input in Inputs)
        {
            if (!InputDataMap[input].IsValid)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Propagates the error of this nodes last output to this nodes inputs
    /// TODO: Elaborate
    /// </summary>
    /// <param name="learningRate">TODO</param>
    protected virtual void BackwardPropagation(double learningRate)
    {
        double errorSignalAggregation = AggregateErrorSignals();

        // Update Bias - Using error signal to be bias gradient
        double biasGradient = errorSignalAggregation; // TODO: Consider using constant to lower this value
        double biasGradientDelta = biasGradient * learningRate;
        Bias += biasGradientDelta;

        // Compute Weight Gradients and Send Input Errors
        foreach (MessageProcessor input in Inputs)
        {
            // Calculate Weight Delta
            double weightGradient = errorSignalAggregation * InputDataMap[input].Value;
            double weightDelta = weightGradient * learningRate;

            // Calculate and Send Input Errors
            double inputErrorSignal = errorSignalAggregation * InputDataMap[input].Weight;
            SendMessage(input, new ErrorSignalUpdateMessage(this, inputErrorSignal));

            // Update Weight
            InputDataMap[input].Weight += weightDelta;
        }
    }

    /// <summary>
    /// The bias added to the result of the Aggregation Function
    /// </summary>
    public double Bias
    {
        get;
        set;
    }

    /// <summary>
    /// TODO
    /// </summary>
    public virtual void Compute()
    {
        // Compute output based on input:
        // - Calculate the scalar product of the input vector and the weight vector
        // - Add a bias to the scalar product
        // - Finally, apply the activation function to the result

        // Apply the Dot Product on the Inputs and their Weight List
        double result = 0;
        foreach (MessageProcessor input in Inputs)
        {
            result += InputDataMap[input].Value * InputDataMap[input].Weight;
        }

        // Add the bias to the Dot Product
        result += Bias;

        // Post-Processing
        LastOutput = PostProcessComputation(result);

        // Send result to outputs
        SendToOutputs(new DataMessage(this, LastOutput));
    }

    /// <summary>
    /// A vector of NeuralNode's that are sending this node outputs.
    /// </summary>
    protected Dictionary<MessageProcessor, InputData> InputDataMap
    {
        get;
        private set;
    }

    protected List<MessageProcessor> Inputs
    {
        get;
        private set;
    }

    private void InvalidateErrorSignals()
    {
        foreach (MessageProcessor output in Outputs)
        {
            OutputDataMap[output].IsValid = false;
        }
    }

    private void InvalidateInputs()
    {
        foreach (MessageProcessor input in Inputs)
        {
            InputDataMap[input].IsValid = false;
        }
    }

    /// <summary>
    /// Stores the last output of this Neuron
    /// </summary>
    public double LastOutput
    {
        get;
        private set;
    }

    /// <summary>
    /// A vector of the NeuralNodes to whcih this node will send messages.
    /// </summary>
    public Dictionary<MessageProcessor, OutputData> OutputDataMap
    {
        get;
        set;
    }

    protected virtual double PostProcessErrorSignalAggregation(double errorSignalAggregation)
    {
        return errorSignalAggregation;
    }

    protected virtual double PostProcessComputation(double computation)
    {
        return computation;
    }

    private void ProcessMessageErrorSignal(ErrorSignalUpdateMessage message)
    {
        int index;
        bool isFound = VectorUtils.GetIndex(Outputs, message.Sender, out index);

        if (isFound)
        {
            OutputDataMap[message.Sender].ErrorSignal = message.ErrorSignal;
            OutputDataMap[message.Sender].IsValid = true;

            if (AreAllErrorSignalsValid())
            {
                BackwardPropagation(LEARNING_RATE);
                InvalidateErrorSignals();
            }
        }
        else
        {
            Logger.Error("Did not process Error Signal message from unregistered output. Sender: " +
                message.Sender);
        }
    }

    private void ProcessInputMessage(DataMessage message)
    {
        int index;
        // TODO: Do I need this?
        bool isFound = VectorUtils.GetIndex<MessageProcessor>(Inputs, message.Sender, out index);

        if (isFound)
        {
            // We just received new information, validate this data.
            InputDataMap[message.Sender].Value = message.Value;
            InputDataMap[message.Sender].IsValid = true;

            if (AreAllInputsValid())
            {
                Compute();
                InvalidateInputs();
            }
        }
        else
        {
            Logger.Error("Did not process Output message from unregistered input. Sender: " +
                message.Sender);
        }
    }

    /// <summary>
    /// Processes an incoming message.
    /// </summary>
    /// <param name="message">Message to process</param>
    public override void ProcessMessage(Message message)
    {
        if (message != null)
        {
            if (message.MessageType == ErrorSignalUpdateMessage.MESSAGE_TYPE)
            {
                ProcessMessageErrorSignal(message as ErrorSignalUpdateMessage);
            }
            else if (message.MessageType == DataMessage.MESSAGE_TYPE)
            {
                ProcessInputMessage(message as DataMessage);
            }
        }
        else
        {
            Logger.Error("Did not process null message.");
        }
    }

    public void RegisterInput(MessageProcessor item, double inputWeight)
    {
        Inputs.Add(item);

        InputData inputData = new InputData();
        inputData.Weight = inputWeight;
        InputDataMap.Add(item, inputData);
    }

    public override void RegisterOutput(MessageProcessor processor)
    {
        OutputData outputData = new OutputData();
        OutputDataMap.Add(processor, outputData);

        base.RegisterOutput(processor);
    }
}

}
}

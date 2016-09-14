using System.Collections.Generic;

namespace LoboLabs
{
namespace NeuralNet
{

using Messaging;

/// <summary>
/// Represents a node in a Neural Network
/// </summary>
public abstract class NeuralNode : MessageProcessor
{
        
    /// <summary>
    /// Constructor
    /// Initializes the number of inputs and outputs to 0. 
    /// </summary>
    public NeuralNode()
    {
        // Outputs
        Outputs = new List<MessageProcessor>();
    }

    protected List<MessageProcessor> Outputs
    {
        get;
        private set;
    }
        
    public virtual void RegisterOutput(MessageProcessor item)
    {
        Outputs.Add(item);
    }

    /// <summary>
    /// Helper function to send the given value to all outputs connected to this Node
    /// </summary>
    /// <param name="value">The value to be sent to outputs</param>
    protected void SendToOutputs(Message message)
    {
        // Send the output to all output nodes
        for (int i = 0; i < Outputs.Count; ++i)
        {
            SendMessage(Outputs[i], message);
        }
    }
}

}
}
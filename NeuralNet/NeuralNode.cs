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
    }
}

}
}
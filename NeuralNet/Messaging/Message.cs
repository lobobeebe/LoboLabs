namespace LoboLabs
{ 
namespace NeuralNet
{
namespace Messaging
{
         
/// <summary>
/// Class representing a generic Message
/// </summary>
public abstract class Message
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="target">Intended recipient of the message</param>
    /// <param name="sender">Sender of the message</param>
    public Message(MessageProcessor sender)
    {
        Sender = sender;
    }

    /// <summary>
    /// Type of the message
    /// </summary>
    public abstract string MessageType
    {
        get;
    }

    /// <summary>
    /// Sender of the message
    /// </summary>
    public MessageProcessor Sender
    {
        get;
        private set;
    }
}

}
}
}

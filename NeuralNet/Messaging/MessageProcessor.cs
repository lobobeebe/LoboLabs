namespace LoboLabs
{

using Utilities;

namespace NeuralNet
{
namespace Messaging
{
         
public abstract class MessageProcessor
{
    private static ClassLogger Logger = new ClassLogger(typeof(MessageProcessor));

    public MessageProcessor()
    {
    }

    /// <summary>
    /// Processes an incoming message.
    /// </summary>
    /// <param name="message">Message to process</param>
    public abstract void ProcessMessage(Message message);

    /// <summary>
    /// Sends a message to target MessageProcessor
    /// </summary>
    /// <param name="target">The intended recipient</param>
    /// <param name="message">The message to be sent</param>
    protected void SendMessage(MessageProcessor target, Message message)
    {
        if (target != null)
        {
            target.ProcessMessage(message);
        }
        else
        {
            Logger.Error("Did not send message to null target. Message: " + message.ToString());
        }
    }
}

}
}
}

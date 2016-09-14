namespace LoboLabs
{
namespace NeuralNet
{

using Messaging;

public class Sensor : NeuralNode
{
    /// <summary>
    /// Constructor
    /// </summary>
    public Sensor()
    {
    }

    public override void ProcessMessage(Message message)
    {
        if(message.MessageType == DataMessage.MESSAGE_TYPE)
        {
            SendToOutputs(new DataMessage(this, (message as DataMessage).Value));
        }
    }
}

}
}
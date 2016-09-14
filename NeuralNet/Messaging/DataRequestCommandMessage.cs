namespace LoboLabs
{ 
namespace NeuralNet
{
namespace Messaging
{
         
public class DataRequestCommandMessage : Message
{
    public const string MESSAGE_TYPE = "COMMAND_DATA_REQUEST";

    public DataRequestCommandMessage(MessageProcessor sender) :
        base(sender)
    {
    }

    public override string MessageType
    {
        get
        {
            return MESSAGE_TYPE;
        }
    }
}

}
}
}

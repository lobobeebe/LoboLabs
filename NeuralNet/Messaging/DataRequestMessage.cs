namespace LoboLabs
{ 
namespace NeuralNet
{
namespace Messaging
{
         
public class DataRequestMessage : Message
{
    public static string MESSAGE_TYPE = "DATA_REQUEST";

    public DataRequestMessage(MessageProcessor sender, int dataType) :
        base(sender)
    {
        DataType = dataType;
    }

    public int DataType
    {
        get;
        private set;
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

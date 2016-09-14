namespace LoboLabs
{ 
namespace NeuralNet
{
namespace Messaging
{
         
public class DataMessage : Message
{
    public const string MESSAGE_TYPE = "INPUT";

    public DataMessage(MessageProcessor sender, double value) :
        base(sender)
    {
        Value = value;
    }
        
    public override string MessageType
    {
        get { return MESSAGE_TYPE;  }
    }

    public double Value
    {
        get;
        private set;
    }
}

}
}
}
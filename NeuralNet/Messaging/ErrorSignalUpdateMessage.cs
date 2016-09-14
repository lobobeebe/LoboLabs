namespace LoboLabs
{ 
namespace NeuralNet
{
namespace Messaging
{
         
public class ErrorSignalUpdateMessage : Message
{
    public const string MESSAGE_TYPE = "ERROR_SIGNAL";

    public ErrorSignalUpdateMessage(MessageProcessor sender, double errorSignal) :
        base(sender)
    {
        ErrorSignal = errorSignal;
    }

    public double ErrorSignal
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

namespace LoboLabs.NeuralNet
{

    public interface GestureScapeListener : ScapeListener
    {
        void ProcessStartGesturing();
        void ProcessStopGesturing();
    }

}
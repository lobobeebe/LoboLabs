using LoboLabs.Utilities;

namespace LoboLabs.NeuralNet
{

    public interface GestureScapeListener
    {
        void ProcessDataUpdate(Vector data);
        void ProcessStartGesturing();
        void ProcessStopGesturing();
    }

}
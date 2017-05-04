namespace LoboLabs.GestureNeuralNet
{
    using Utilities;

    public interface GestureDataReceiver
    {
        void StartGesturing();

        void StopGesturing();

        void UpdateGesturePosition(Vector position);
    }
}

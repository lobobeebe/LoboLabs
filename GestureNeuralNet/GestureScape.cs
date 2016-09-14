using LoboLabs.NeuralNet;
using LoboLabs.Utilities;

namespace LoboLabs.GestureNeuralNet
{

    public class GestureScape : Scape
    {
        private static ClassLogger Logger = new ClassLogger(typeof(GestureScape));

        private Vector mLastPosition;
        private bool mHasLastPosition;

        public GestureScape()
        {
            mLastPosition = new Vector(0, 0, 0);
        }

        public virtual void StartGesturing()
        {
            mHasLastPosition = false;
        }

        public virtual void StopGesturing()
        {
            // Do nothing
        }

        public virtual void UpdateGesturePosition(Vector position)
        {
            // If there isn't a saved position, set the last position to the current position, effectively inputting a zero vector
            if(!mHasLastPosition)
            {
                mLastPosition = position;
            }

            // Notify listeners of the new relative position
            NotifyListeners((position - mLastPosition).ToList());

            // Update last position and set has position
            mLastPosition = position;
            mHasLastPosition = true;
        }
    }

}
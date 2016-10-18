using System.Collections.Generic;

using LoboLabs.NeuralNet;
using LoboLabs.NeuralNet.Functions;
using LoboLabs.Utilities;

namespace LoboLabs.GestureNeuralNet
{

    public class BinaryGestureNeuralNetwork : NeuralNetwork, GestureScapeListener
    {
        private const double GESTURE_CONFIDENCE_THRESHOLD = 0.75;
        private const string DEFAULT_NAME = "Default Gesture";

        private static ClassLogger Logger = new ClassLogger(typeof(BinaryGestureNeuralNetwork));

        public string Name
        {
            get;
            set;
        }

        public BinaryGestureNeuralNetwork()
        {
            // List of Listeners
            Listeners = new List<GestureDetectionListener>();

            Name = DEFAULT_NAME;
        }

        private List<GestureDetectionListener> Listeners
        {
            get;
            set;
        }

        private void NotifyGestureDetection()
        {
            Logger.Debug("Gesture Detected: " + Name);
            foreach (GestureDetectionListener listener in Listeners)
            {
                listener.OnGestureDetected(Name);
            }
        }

        protected override void OnResult(double result)
        {
            // If the result is sufficient, notify listeners
            if (result > GESTURE_CONFIDENCE_THRESHOLD)
            {
                NotifyGestureDetection();
            }
            else
            {
                Logger.Debug("Result not sufficient: " + result);
            }

        }

        public void RegisterListener(GestureDetectionListener listener)
        {
            Listeners.Add(listener);
        }

        public void ProcessStartGesturing()
        {
        }

        public void ProcessStopGesturing()
        {
        }
    }

}
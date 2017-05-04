using LoboLabs.Utilities;
using System.Collections.Generic;
using System.IO;

namespace LoboLabs.GestureNeuralNet
{
    /// <summary>
    /// Allows the User to save and load Gesture Definitions from disk
    /// </summary>
    public class GestureManager
    {
        public delegate void GestureDetectionHandler(GestureHand gestureHand, string gestureName);
        public event GestureDetectionHandler OnGestureDetected;

        private Dictionary<GestureHand, GestureDetector> mDetectors;

        public GestureManager()
        {
            Name = "GestureManager";
            mDetectors = new Dictionary<GestureHand, GestureDetector>();

            //GestureDetector rightGestureDetector = new GestureDetector();
            //rightGestureDetector.OnGestureDetected += OnRightGestureDetected;
            //mDetectors.Add(GestureHand.RIGHT, rightGestureDetector);
        }

        public string Name
        {
            get;
            set;
        }

        public void OnRightGestureDetected(string gestureName)
        {
            OnGestureDetected(GestureHand.RIGHT, gestureName);
        }

        public void Save(BinaryWriter writer)
        {
            // Write the name of the Manager
            writer.Write(Name);

            // Write number of items in the Dictionary
            writer.Write(mDetectors.Count);

            foreach (KeyValuePair<GestureHand, GestureDetector> pair in mDetectors)
            {
                // Write the value of the GestureHand
                writer.Write((int)pair.Key);
                    
                // Write the key
                pair.Value.Save(writer);
            }
        }

        public void StartGesturing(GestureHand gestureHand)
        {
            mDetectors[gestureHand]?.StartGesturing();
        }
        
        public void StopGesturing(GestureHand gestureHand)
        {
            mDetectors[gestureHand]?.StopGesturing();
        }

        public void UpdateGesturePosition(GestureHand gestureHand, Vector vector)
        {
            mDetectors[gestureHand]?.UpdateGesturePosition(vector);
        }
    }
}

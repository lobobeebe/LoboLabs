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
        public event GestureDetectionHandler GestureDetected;

        private Dictionary<GestureHand, GestureDetector> mDetectors;

        public GestureManager()
        {
            Name = "GestureManager";
            mDetectors = new Dictionary<GestureHand, GestureDetector>();
        }

        private void Load(BinaryReader reader)
        {
            // Read the name of the Manager
            Name = reader.ReadString();

            // Read the number of Gesture Detectors
            int numDetectors = reader.ReadInt32();
            mDetectors = new Dictionary<GestureHand, GestureDetector>();

            // Read in the contents of the Hand, Detector dictionary
            for (int i = 0; i < numDetectors; ++i)
            {
                GestureHand hand = (GestureHand)reader.ReadInt32();
                GestureDetector detector = new GestureDetector(reader);

                SetDetector(hand, detector);
            }
        }

        public string Name
        {
            get;
            set;
        }

        public void OnLeftGestureDetected(string gestureName)
        {
            GestureDetected(GestureHand.LEFT, gestureName);
        }

        public void OnRightGestureDetected(string gestureName)
        {
            GestureDetected(GestureHand.RIGHT, gestureName);
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

        public void SetDetector(GestureHand hand, GestureDetector detector)
        {
            mDetectors[hand] = detector;

            switch (hand)
            {
                case GestureHand.LEFT:
                    detector.GestureDetected += OnLeftGestureDetected;
                    break;
                case GestureHand.RIGHT:
                    detector.GestureDetected += OnRightGestureDetected;
                    break;
                default:
                    break;
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

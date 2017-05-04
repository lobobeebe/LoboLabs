using System.Collections.Generic;

namespace LoboLabs.GestureNeuralNet
{
    using NeuralNet;
    using NeuralNet.Functions;
    using System.IO;
    using Utilities;

    public enum GestureHand
    {
        RIGHT,
        LEFT
    }

    public class GestureDetector : GestureDataReceiver
    {
        private const string GESTURE_EXTENSION = ".gc";

        private const float DEFAULT_THRESHOLD = 0.9f;

        public delegate void GestureDetectionHandler(string gestureName);
        public event GestureDetectionHandler OnGestureDetected;

        private GestureScape mScape;
        private NeuralNetwork mNetwork;

        private List<string> mGestureNames;

        public GestureDetector(BinaryReader reader)
        {
            Load(reader);
        }

        public GestureDetector(NeuralNetwork neuralNet, List<string> gestureNames)
        {
            mGestureNames = gestureNames;

            mNetwork = neuralNet;
            mNetwork.ResultComputed += OnResultComputed;

            mScape = new GestureScape((uint)mNetwork.Sensors.Count / 3);
            mScape.DataReceived += OnDataReceived;

            MinThreshold = DEFAULT_THRESHOLD;
        }

        public bool Equals(GestureDetector other)
        {
            bool isEqual = false;

            if (mNetwork.Equals(other.mNetwork) &&
                mGestureNames.Equals(other.mGestureNames) &&
                MinThreshold.Equals(other.MinThreshold) &&
                mScape.Equals(other.mScape))
            {
                isEqual = true;
            }


            return isEqual;
        }

        public NeuralNetwork GetNetwork()
        {
            return mNetwork;
        }

        public List<string> GetGestureNames()
        {
            return mGestureNames;
        }

        private void Load(BinaryReader reader)
        {
            // Read the Network
            mNetwork = new NeuralNetwork(reader);
            mNetwork.ResultComputed += OnResultComputed;

            // Create the Scape
            mScape = new GestureScape((uint)mNetwork.Sensors.Count / 3);
            mScape.DataReceived += OnDataReceived;

            // Read the Min Threshold
            MinThreshold = reader.ReadSingle();

            // Read the number of gesture Classes
            int numGestures = reader.ReadInt32();

            mGestureNames = new List<string>();
            // Read the Gesture Names
            for (int i = 0; i < numGestures; ++i)
            {
                string name = reader.ReadString();
                mGestureNames.Add(name);
                //GestureClass gestureClass = new GestureClass(); 
                //gestureClass.LoadFromFile(gestureDirectory + "/" + name + GESTURE_EXTENSION);
            }
        }
        
        public float MinThreshold
        {
            get;
            set;
        }

        /*
        public void LoadAndTrainNetwork(string gestureDirectory)
        {
            // Load the Training Data from a file
            LoadGesturesFromPath(gestureDirectory);

            CompleteTraining();
        }

        public List<string> LoadGesturesFromPath(string directory)
        {
            List<string> nameList = new List<string>();
            List<DataClass> definitionList = new List<DataClass>();

            if (Directory.Exists(directory))
            {
                ClearGestures();

                string[] filesInDirectory = Directory.GetFiles(directory, "*" + GESTURE_EXTENSION);

                foreach (string file in filesInDirectory)
                {
                    GestureClass definition = new GestureClass();
                    definition.LoadFromFile(file);

                    if (definition != null)
                    {
                        definitionList.Add(definition);
                        nameList.Add(definition.Name);
                    }
                }
            }

            mClassList = definitionList;

            return nameList;
        }
        */
        public void OnResultComputed(ScapeData input, List<double> output)
        {
            string outputName = "";

            for (int outputIndex = 0; outputIndex < output.Count; ++outputIndex)
            {
                if (output[outputIndex] > MinThreshold)
                {
                    outputName = mGestureNames[outputIndex];
                }
            }

            if (outputName.Length > 0)
            {
                OnGestureDetected(outputName);
            }
        }

        public void OnDataReceived(ScapeData scapeData)
        {
            mNetwork.ProcessData(scapeData);
        }

        public void Save(BinaryWriter writer)
        {
            // Write out the Network
            mNetwork.Save(writer);

            // Write the Minimum Threshold to detect a gesture
            writer.Write(MinThreshold);

            // Write the number of gesture classes
            writer.Write(mGestureNames.Count);

            // Write the names of the definitions
            foreach (string gestureName in mGestureNames)
            {
                writer.Write(gestureName);
            }
        }

        /*
        public void SaveGesturesToPath(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            foreach (DataClass definition in mClassList)
            {
                definition.SaveToFile(directory + "\\" + definition.Name + GESTURE_EXTENSION);
            }
        }
        */

        public void StartGesturing()
        {
            mScape.StartGesturing();
        }

        public void StopGesturing()
        {
            mScape.StopGesturing();
        }

        public void UpdateGesturePosition(Vector gesturePosition)
        {
            mScape.UpdateGesturePosition(gesturePosition);
        }
    }
}

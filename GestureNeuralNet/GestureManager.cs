using System.Collections.Generic;
using System.IO;

namespace LoboLabs.GestureNeuralNet
{
    using NeuralNet;
    using NeuralNet.Functions;
    using Utilities;

    /// <summary>
    /// Allows the User to save and load Gesture Definitions from disk
    /// </summary>
    public class GestureManager
    {
        public delegate void GestureDetectionHandler(object sender, string gestureName);
        public event GestureDetectionHandler OnGestureDetected;

        private const string GESTURE_EXTENSION = ".gc";
        private const int NUM_DIRECTIONAL_VECTORS = 10;

        private GestureScape mScape;
        private NeuralNetworkTrainer mTrainer;
        private NeuralNetwork mNetwork;
        
        private List<DataClass> mClassList;
        private List<string> mGestureNames;

        public GestureManager()
        {
            mTrainer = new NeuralNetworkTrainer(new SumSquaredError(), NUM_DIRECTIONAL_VECTORS * 3);
            mTrainer.LearningRate = .2;

            mScape = new GestureScape(NUM_DIRECTIONAL_VECTORS);
            mScape.DataReceived += ProcessData;

            mClassList = new List<DataClass>();

            IsTraining = true;
            Threshold = 0.75;
        }

        public void CompleteTraining()
        {
            mGestureNames = DataClass.GetNamesFromList(mClassList);

            // Generate a Gesture NeuralNet with three hidden nodes
            NeuralNetworkGenerator generator = new NeuralNetworkGenerator();
            generator.NumHidden = 3;
            mNetwork = generator.Generate(
                NUM_DIRECTIONAL_VECTORS * 3, // Number of vectors * 3 (x, y, z)
                mGestureNames);

            // Train networks
            mTrainer.TrainBackPropagation(mNetwork, mClassList);

            mNetwork.ResultComputed += OnResultComputed;
            
            IsTraining = false;
        }

        public string CurrentGestureName
        {
            get;
            set;
        }

        private GestureClass GetGestureByName(string name)
        {
            GestureClass returnGesture = null;
            foreach (GestureClass gesture in mClassList)
            {
                if (gesture.Name == name)
                {
                    returnGesture = gesture;
                }
            }

            // If not found, create a new one and add it to the Definitions list
            if (returnGesture == null)
            {
                returnGesture = new GestureClass(name, NUM_DIRECTIONAL_VECTORS);
                mClassList.Add(returnGesture);
            }

            return returnGesture;
        }

        public bool IsTraining
        {
            get;
            set;
        }

        public void Load(string gestureDirectory)
        {
            // Load the Training Data from a file
            mClassList = LoadGesturesFromPath(gestureDirectory);

            CompleteTraining();
        }

        public static List<DataClass> LoadGesturesFromPath(string directory)
        {
            List<DataClass> definitionList = new List<DataClass>();

            if (Directory.Exists(directory))
            {
                string[] filesInDirectory = Directory.GetFiles(directory, "*" + GESTURE_EXTENSION);

                foreach (string file in filesInDirectory)
                {
                    GestureClass definition = new GestureClass();
                    definition.LoadFromFile(file);

                    if (definition != null)
                    {
                        definitionList.Add(definition);
                    }
                }
            }
            
            return definitionList;
        }

        public void OnResultComputed(object sender, ScapeData input, List<double> output)
        {
            string outputName = "";

            for (int outputIndex = 0; outputIndex < output.Count; ++outputIndex)
            {
                if (output[outputIndex] > Threshold)
                {
                    outputName = mGestureNames[outputIndex];
                }
            }

            if (outputName.Length > 0)
            {
                OnGestureDetected(this, outputName);
            }
        }
        
        public void ProcessData(object sender, ScapeData scapeData)
        {
            if (IsTraining)
            {
                // Find or create the current Data Definition
                GestureClass currentDefinition = GetGestureByName(CurrentGestureName);

                currentDefinition.AddScapeData(scapeData);
            }
            else
            {
                mNetwork.ProcessData(sender, scapeData);
            }
        }

        public static List<DataClass> SaveGesturesToPath(string directory, List<DataClass> definitionList)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            foreach (DataClass definition in definitionList)
            {
                definition.SaveToFile(directory + "\\" + definition.Name + GESTURE_EXTENSION);
            }

            return definitionList;
        }

        public void StartGesturing()
        {
            mScape.StartGesturing();
        }

        public void StopGesturing()
        {
            mScape.StopGesturing();
        }

        public double Threshold
        {
            get;
            set;
        }
        
        public void UpdateGesturePosition(Vector gesturePosition)
        {
            mScape.UpdateGesturePosition(gesturePosition);
        }
    }
}

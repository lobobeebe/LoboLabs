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

        private const uint DEFAULT_NUM_REQUIRED_VECTORS = 30;
        private const string GESTURE_EXTENSION = ".gc";

        private GestureScape mScape;
        private NeuralNetwork mNetwork;
        
        private List<DataClass> mClassList;
        private List<string> mGestureNames;

        public GestureManager()
        {
            NumRequiredVectors = DEFAULT_NUM_REQUIRED_VECTORS;

            mScape = new GestureScape(NumRequiredVectors);
            mScape.DataReceived += ProcessData;

            mClassList = new List<DataClass>();

            IsTraining = true;
            Threshold = 0.9;
        }

        public void ClearGestures()
        {
            mClassList = new List<DataClass>();
            mGestureNames = new List<string>();
            CurrentGestureName = "";
        }

        public void CompleteTraining()
        {
            mGestureNames = DataClass.GetNamesFromList(mClassList);

            // Generate a Gesture NeuralNet with three hidden nodes
            NeuralNetworkGenerator generator = new NeuralNetworkGenerator();
            generator.NumHidden = 3;
            mNetwork = generator.Generate(
                (int)NumRequiredVectors * 3, // Number of vectors * 3 (x, y, z)
                mGestureNames);

            // Train networks
            NeuralNetworkTrainer trainer = new NeuralNetworkTrainer(new SumSquaredError(), NumRequiredVectors * 3);
            trainer.LearningRate = .2;
            trainer.TrainBackPropagation(mNetwork, mClassList);

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
                returnGesture = new GestureClass(name, NumRequiredVectors);
                mClassList.Add(returnGesture);
            }

            return returnGesture;
        }

        public bool IsTraining
        {
            get;
            set;
        }

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

        public uint NumRequiredVectors
        {
            get;
            set;
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

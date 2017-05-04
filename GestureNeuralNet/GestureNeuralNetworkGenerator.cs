using LoboLabs.NeuralNet;
using LoboLabs.NeuralNet.Functions;
using LoboLabs.Utilities;
using System.Collections.Generic;

namespace LoboLabs.GestureNeuralNet
{
    public class GestureNeuralNetworkGenerator : GestureDataReceiver
    {
        private const uint DEFAULT_NUM_INPUT_VECTORS = 20;

        private List<DataClass> mClassList;
        private GestureScape mScape;

        public GestureNeuralNetworkGenerator()
        {
            NumInputVectors = DEFAULT_NUM_INPUT_VECTORS;

            mClassList = new List<DataClass>();

            mScape = new GestureScape(NumInputVectors);
            mScape.DataReceived += OnDataReceived;
        }

        public void ClearGestures()
        {
            mClassList = new List<DataClass>();
            CurrentGestureName = "";
        }

        public string CurrentGestureName
        {
            get;
            set;
        }

        public NeuralNetwork Generate()
        {
            // Generate a Gesture NeuralNet
            NeuralNetworkCreator creator = new NeuralNetworkCreator();
            NeuralNetwork network = creator.Create(NumInputData, mClassList.Count);

            // Train network
            NeuralNetworkTrainer trainer = new NeuralNetworkTrainer(new SumSquaredError(), NumInputData);
            trainer.TrainBackPropagation(network, mClassList);

            return network;
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
                returnGesture = new GestureClass(name, NumInputVectors);
                mClassList.Add(returnGesture);
            }

            return returnGesture;
        }

        public List<string> GetGestureNames()
        {
            return DataClass.GetNamesFromList(mClassList);
        }

        private uint NumInputData
        { 
            // Number of vectors * 3 (x, y, z)
            get
            {
                return NumInputVectors * 3;
            }
        }

        public uint NumInputVectors
        {
            get;
            set;
        }
        
        public void OnDataReceived(ScapeData scapeData)
        {
            // Find or create the current Data Definition
            GestureClass currentDefinition = GetGestureByName(CurrentGestureName);

            currentDefinition.AddScapeData(scapeData);
        }

        public void StartGesturing()
        {
            mScape.StartGesturing();
        }

        public void StopGesturing()
        {
            mScape.StopGesturing();
        }

        public void UpdateGesturePosition(Vector position)
        {
            mScape.UpdateGesturePosition(position);
        }
    }
}

using System.Collections.Generic;
using LoboLabs.NeuralNet;
using LoboLabs.Utilities;

namespace LoboLabs.GestureNeuralNet
{
    
    public class GestureTrainer : GestureScapeListener
    {
        public GestureTrainer()
        {
            TrainingCategory = "Default";
            TrainingGestureNames = new List<string>();
            TrainingGestures = new Dictionary<string, List<List<Vector>>>();
        }

        private List<KnownData> CompileKnownData()
        {
            List<KnownData> trainingData = new List<KnownData>();

            for (int categoryIndex = 0; categoryIndex < TrainingGestureNames.Count; ++categoryIndex)
            {
                // Create an expected output vector as a One-Of-N vector corresponding to
                // the correct output
                List<double> expectedOutput = new List<double>(TrainingGestureNames.Count);
                for(int slot = 0; slot < TrainingGestureNames.Count; ++slot)
                {
                    if(slot == categoryIndex)
                    {
                        expectedOutput.Add(1);
                    }
                    else
                    {
                        expectedOutput.Add(0);
                    }
                }

                // Now that we have an expected output, let's create all the inputs that map to it.
                string category = TrainingGestureNames[categoryIndex];
                // This list contains a list of lists of doubles. Each list of doubles is a position.
                // Together, in a sequence, they are a gesture.
                List<List<double>> gestureList = TrainingGestures[category];

                for(int gestureIndex = 0; gestureIndex < gestureList.Count; ++gestureIndex)
                {
                    trainingData.Add(new KnownData(gestureList[gestureIndex], expectedOutput));
                }
            }

            return trainingData;
        }

        public void ProcessDataUpdate(Vector data)
        {
            TrainingGestures[TrainingCategory].Add(data);
        }

        public void Train(StaticGestureNeuralNetwork network)
        {
            List<KnownData> trainingData = CompileKnownData();

            // TODO: Reconsider Magic Numbers
            network.GestureNames = TrainingGestureNames;
            network.TrainBackPropagation(trainingData, 100, 0.05);
        }

        public void ProcessStartGesturing()
        {
            if (!TrainingGestureNames.Contains(TrainingCategory))
            {
                TrainingGestureNames.Add(TrainingCategory);
                TrainingGestures.Add(TrainingCategory, new List<List<double>>());
            }
        }

        public void ProcessStopGesturing()
        {

        }

        public string TrainingCategory
        {
            get;
            set;
        }

        public List<string> TrainingGestureNames
        {
            get;
            private set;
        }

        private Dictionary<string, List<List<Vector>>> TrainingGestures
        {
            get;
            set;
        }
    }

}
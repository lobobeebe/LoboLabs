using System.Collections.Generic;

namespace LoboLabs
{

using NeuralNet;
using Utilities;

namespace GestureNeuralNet
{

public class StaticGestureTrainer : ScapeListener
{
    public StaticGestureTrainer()
    {
        TrainingCategory = "Hello";
        TrainingDataNames = new List<string>();
        TrainingDataValues = new Dictionary<string, List<List<double>>>();
    }

    private List<KnownData> CompileKnownData()
    {
        List<KnownData> trainingData = new List<KnownData>();

        for (int categoryIndex = 0; categoryIndex < TrainingDataNames.Count; ++categoryIndex)
        {
            // Create an expected output vector as a One-Of-N vector corresponding to
            // the correct output
            List<double> expectedOutput = new List<double>(TrainingDataNames.Count);
            for(int slot = 0; slot < TrainingDataNames.Count; ++slot)
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
            string category = TrainingDataNames[categoryIndex];
            // This list contains a list of lists of doubles. The lists of doubles are gestures.
            // ie. This is a list of gestures.
            List<List<double>> gestureList = TrainingDataValues[category];

            for(int gestureIndex = 0; gestureIndex < gestureList.Count; ++gestureIndex)
            {
                trainingData.Add(new KnownData(gestureList[gestureIndex], expectedOutput));
            }
        }

        return trainingData;
    }

    public void OnReceiveScapeData(List<double> data)
    {
        if(!TrainingDataNames.Contains(TrainingCategory))
        {
            TrainingDataNames.Add(TrainingCategory);
            TrainingDataValues.Add(TrainingCategory, new List<List<double>>());
        }

        TrainingDataValues[TrainingCategory].Add(new List<double>(data));
    }

    public void Train(StaticGestureNeuralNetwork network)
    {
        List<KnownData> trainingData = CompileKnownData();

        // TODO: Reconsider Magic Numbers
        network.GestureNames = TrainingDataNames;
        network.TrainBackPropagation(trainingData, 100, 0.05);
    }

    public string TrainingCategory
    {
        get;
        set;
    }

    public List<string> TrainingDataNames
    {
        get;
        private set;
    }

    private Dictionary<string, List<List<double>>> TrainingDataValues
    {
        get;
        set;
    }
}

}
}
using System.Collections.Generic;

namespace LoboLabs
{

using NeuralNet;
using Utilities;

namespace GestureNeuralNet
{

public class StaticGestureScape : Scape
{
    private static ClassLogger Logger = new ClassLogger(typeof(StaticGestureScape));

    private const int NUM_VECTORS = 10;

    public StaticGestureScape()
    {
        CurrentData = new List<Vector>();
    }

    private List<Vector> CurrentData
    {
        get;
        set;
    }

    public void EndGesture()
    {
        if (CurrentData.Count >= NUM_VECTORS)
        {
            // Pair down the gesture to NUM_INPUTS vectors
            List<double> EstimatedGesture = new List<double>();
            // Add the first location in the gesture
            EstimatedGesture.Add(CurrentData[0].x);
            EstimatedGesture.Add(CurrentData[0].x);
            EstimatedGesture.Add(CurrentData[0].x);

            // Add estimated middle points
            for (int i = 0; i < CurrentData.Count; i += CurrentData.Count / NUM_VECTORS)
            {
                EstimatedGesture.Add(CurrentData[i].x);
                EstimatedGesture.Add(CurrentData[i].y);
                EstimatedGesture.Add(CurrentData[i].z);
            }
            // Be sure to add the last data point in the gesture
            if (EstimatedGesture.Count < NUM_VECTORS)
            {
                EstimatedGesture.Add(CurrentData[CurrentData.Count - 1].x);
                EstimatedGesture.Add(CurrentData[CurrentData.Count - 1].y);
                EstimatedGesture.Add(CurrentData[CurrentData.Count - 1].z);
            }
                
            // Notify listeners of the gesture
            NotifyListeners(EstimatedGesture);
        }
        else
        {
            Logger.Debug("Input Gesture Too Short.");
        }

        CurrentData.Clear();
    }

    public void UpdateGesturePosition(Vector position)
    {
        CurrentData.Add(position);
    }
}

}
}
using System.Collections.Generic;

namespace LoboLabs
{

using NeuralNet;
using NeuralNet.Functions;
using Utilities;

namespace GestureNeuralNet
{

public class StaticGestureNeuralNetwork : NeuralNetwork
{
    public StaticGestureNeuralNetwork() :
        base(new SoftmaxFunction(), new SumSquaredError())
    {
        // List of Listeners
        Listeners = new List<GestureDetectionListener>();
    }

    private const double GESTURE_CONFIDENCE_THRESHOLD = 0.75;

    private List<GestureDetectionListener> Listeners
    {
        get;
        set;
    }

    protected override void OnResult(List<double> result)
    {
        int maxIndex = VectorUtils.MaxIndex(result);
            
        if(result[maxIndex] > GESTURE_CONFIDENCE_THRESHOLD)
        {
            foreach (GestureDetectionListener listener in Listeners)
            {
                listener.OnGestureDetected(GestureNames[maxIndex]);
            }
        }
    }

    public void RegisterListener(GestureDetectionListener listener)
    {
        Listeners.Add(listener);
    }

    public List<string> GestureNames
    {
        get;
        set;
    }
}

}
}
using System.Collections.Generic;

namespace LoboLabs
{ 
namespace NeuralNet
{

/// <summary>
/// Represents that Data that will be piped into a Neural Net
/// </summary>
public abstract class Scape
{
    public Scape()
    {
        ScapeListeners = new List<ScapeListener>();
    }

    public void RegisterListener(ScapeListener listener)
    {
        ScapeListeners.Add(listener);
    }

    public void RemoveListener(ScapeListener listener)
    {
        ScapeListeners.Remove(listener);
    }

    private List<ScapeListener> ScapeListeners
    {
        get;
        set;
    }

    public void NotifyListeners(List<double> data)
    {
        foreach(ScapeListener listener in ScapeListeners)
        {
            listener.ProcessData(data);
        }
    }
}

}
}
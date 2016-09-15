using System.Collections.Generic;

namespace LoboLabs
{ 
namespace NeuralNet
{

public interface ScapeListener
{
    void ProcessData(List<double> data);
}

}
}
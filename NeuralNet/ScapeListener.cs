using System.Collections.Generic;

namespace LoboLabs
{ 
namespace NeuralNet
{

public interface ScapeListener
{
    void OnReceiveScapeData(List<double> data);
}

}
}
using System.IO;
using LoboLabs.NeuralNet;

namespace LoboLabs.GestureNeuralNet
{
    public class GestureClass : DataClass
    {
        public GestureClass(string name = "", uint numInputs = 0) : base(name, numInputs)
        {
        }
        
        protected override ScapeData LoadDataFromStream(BinaryReader reader)
        {
            GestureData gestureData = new GestureData(NumInputs);
            gestureData.LoadFromStream(reader);

            return gestureData;
        }
    }
}

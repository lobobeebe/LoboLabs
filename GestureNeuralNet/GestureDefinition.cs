using System;
using System.Collections.Generic;
using System.IO;
using LoboLabs.NeuralNet;

namespace LoboLabs.GestureNeuralNet
{
    public class GestureDefinition : ScapeDataDefinition
    {
        public GestureDefinition(string name, int numInputs) : base(name, numInputs)
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

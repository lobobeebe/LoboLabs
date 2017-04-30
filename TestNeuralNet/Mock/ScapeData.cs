using System.Collections.Generic;
using System.IO;

namespace LoboLabs.NeuralNet.Mock
{
    public class ScapeData : NeuralNet.ScapeData
    {
        public List<double> AsList()
        {
            return new List<double>();
        }

        public void LoadFromStream(BinaryReader reader)
        {
        }

        public void WriteToStream(BinaryWriter writer)
        {
        }
    }
}

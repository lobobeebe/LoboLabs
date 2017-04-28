using System.Collections.Generic;
using System.IO;

namespace LoboLabs.NeuralNet
{ 
    /// <summary>
    /// TODO: Documentation
    /// TODO: Unit Tests
    /// </summary>
    public interface ScapeData
    {
        List<double> AsList();

        void LoadFromStream(BinaryReader reader);

        void WriteToStream(BinaryWriter writer);
    }
}

using System.Collections.Generic;

namespace LoboLabs.Utilities
{ 

    /// <summary>
    /// TODO: Documentation
    /// TODO: Unit Tests
    /// </summary>
    public class TrainingData<Input> where Input : NetworkInputType
    {
        public TrainingData(List<Input> inputs, List<double> expectedOutputs)
        {
            InputValues = inputs;
            ExpectedOutputs = expectedOutputs;
        }

        public List<Input> InputValues
        {
            get;
            set;
        }

        public List<double> ExpectedOutputs
        {
            get;
            set;
        }
    }

}

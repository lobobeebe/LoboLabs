using System.Collections.Generic;

namespace LoboLabs.Utilities
{

    public class DoublesPair : List<double>, NetworkInputType
    {
        public DoublesPair()
        {
        }

        public DoublesPair(params double[] doubles)
        {
            foreach(double value in doubles)
            {
                Add(value);
            }
        }

        public List<double> ToList()
        {
            return this;
        }
    }
}
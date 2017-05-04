namespace LoboLabs.Utilities
{
    public class WeightData
    {        
        public bool Equals(WeightData other)
        {
            return Weight == other.Weight;
        }

        public double Weight
        {
            get;
            set;
        }

        public double TotalWeightDelta
        {
            get;
            set;
        }
    }
}
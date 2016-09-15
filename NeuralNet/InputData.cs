namespace LoboLabs.NeuralNet.Messaging
{

    public class InputData
    {
        public bool IsValid
        {
            get;
            set;
        }

        public double Value
        {
            get;
            set;
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
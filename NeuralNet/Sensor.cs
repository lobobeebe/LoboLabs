using System;

namespace LoboLabs.NeuralNet
{

    public class Sensor : ComputationalNode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Sensor() : base(new Functions.NoOpActivationFunction())
        {
        }

        public override void CalculateErrorSignals(double learningRate)
        {
            // Cannot Compute so there will never be error signals
            throw new NotImplementedException();
        }

        public override void Compute()
        {
            // Cannot Compute with a Sensor
            throw new NotImplementedException();
        }

        public void SetLastOutput(double lastOutput)
        {
            LastOutput = lastOutput;
        }
    }
}

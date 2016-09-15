namespace LoboLabs.NeuralNet
{

    /// <summary>
    /// Represents a final node in a Neural Net.
    /// This class has an interface through which a supervisor can retrieve final output.
    /// TODO: Unit Test
    /// </summary>
    public class Actuator : ComputationalNode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Actuator() : base(new Functions.NoOpActivationFunction())
        {
        }
    }

}
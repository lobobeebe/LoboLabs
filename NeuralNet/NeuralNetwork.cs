using System.Collections.Generic;

using LoboLabs.Utilities;
namespace LoboLabs.NeuralNet
{

    /// <summary>
    /// Represents a generic NeuralNetwork
    /// </summary>
    public abstract class NeuralNetwork : ScapeListener
    {
        private static ClassLogger Logger = new ClassLogger(typeof(NeuralNetwork));

        /// <summary>
        /// Constructor
        /// </summary>
        public NeuralNetwork()
        {
            Sensors = new List<Node>();
            Neurons = new List<List<ComputationalNode>>();
        }

        public ComputationalNode GetActuator()
        {
            // TODO: Change to return multiple Actuators
            ComputationalNode actuator = null;

            if(Neurons != null && Neurons.Count > 0)
            {
                List<ComputationalNode> actuators = Neurons[Neurons.Count - 1];
                actuator = actuators[0];
            }
            else
            {
                Logger.Error("Cannot get actuators from empty network.");
            }

            return actuator;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public double Compute(List<double> inputs)
        {
            double output = 0;

            if (Neurons.Count > 0 && Neurons[Neurons.Count - 1].Count > 0 && inputs != null)
            {
                if (inputs.Count == Sensors.Count)
                {
                    // Set input information of Sensors
                    for (int i = 0; i < Sensors.Count; ++i)
                    {
                        Sensors[i].LastOutput = inputs[i];
                    }

                    // Compute for all nodes in order of layer
                    for (int layerIndex = 0; layerIndex < Neurons.Count; ++layerIndex)
                    {
                        for (int nodeIndex = 0; nodeIndex < Neurons[layerIndex].Count; ++nodeIndex)
                        {
                            Neurons[layerIndex][nodeIndex].Compute();
                        }
                    }

                    // TODO: Change this when multiple actuators are allowed
                    List<ComputationalNode> actuators = Neurons[Neurons.Count - 1];
                    output = actuators[0].LastOutput;
                }
                else
                {
                    Logger.Error("Cannot Compute inputs of size " + inputs.Count + " with " + Sensors.Count + " Sensors.");
                }
            }
            else
            {
                Logger.Error("Cannot Compute: {Inputs: " + inputs + ", Neurons: " + Neurons + "}");
            }

            return output;
        }

        /// <summary>
        /// Vector of Neurons that belong to this Neural Net
        /// </summary>
        public List<List<ComputationalNode>> Neurons
        {
            get;
            set;
        }

        public void ProcessData(List<double> data)
        {
            // Compute on the input data
            OnResult(Compute(data));
        }

        protected abstract void OnResult(double result);
        
        /// <summary>
        /// Vector of sensors that belong to this Neural Net
        /// </summary>
        public List<Node> Sensors
        {
            get;
            set;
        }

        public void WriteToFile(string fileName)
        {
            // TODO
        }
    }

}

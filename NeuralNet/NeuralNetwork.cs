using System.Collections.Generic;

using LoboLabs.Utilities;
namespace LoboLabs.NeuralNet
{
    /// <summary>
    /// Represents a generic NeuralNetwork
    /// </summary>
    public class NeuralNetwork
    {
        private static ClassLogger Logger = new ClassLogger(typeof(NeuralNetwork));

        public delegate void NeuralNetworkResultHandler(object sender, ScapeData input, List<double> output);
        public event NeuralNetworkResultHandler ResultReceived;

        /// <summary>
        /// Constructor
        /// </summary>
        public NeuralNetwork()
        {
            Sensors = new List<Node>();
            Neurons = new List<List<ComputationalNode>>();
        }

        public List<ComputationalNode> GetActuators()
        {
            List<ComputationalNode> actuators = null;

            if(Neurons != null && Neurons.Count > 0)
            {
                actuators = Neurons[Neurons.Count - 1];
            }
            else
            {
                Logger.Error("Cannot get actuators from empty network.");
            }

            return actuators;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public List<double> Compute(List<double> inputs)
        {
            List<double> outputs = new List<double>();

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
                    
                    List<ComputationalNode> actuators = GetActuators();
                    for (int actuatorIndex = 0; actuatorIndex < actuators.Count; ++actuatorIndex)
                    {
                        outputs.Add(actuators[actuatorIndex].LastOutput);
                    }
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

            return outputs;
        }

        /// <summary>
        /// Vector of Neurons that belong to this Neural Net
        /// </summary>
        public List<List<ComputationalNode>> Neurons
        {
            get;
            set;
        }

        public void ProcessData(object sender, ScapeData scapeData)
        {
            List<double> data = scapeData.AsList(); 
            ResultReceived(this, scapeData, Compute(data));
        }
        
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

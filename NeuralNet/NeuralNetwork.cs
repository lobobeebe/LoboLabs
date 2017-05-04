using System.Collections.Generic;

using LoboLabs.Utilities;
using System.IO;

namespace LoboLabs.NeuralNet
{
    /// <summary>
    /// Represents a generic NeuralNetwork
    /// </summary>
    public class NeuralNetwork
    {
        private static ClassLogger Logger = new ClassLogger(typeof(NeuralNetwork));

        public delegate void NeuralNetworkResultHandler(ScapeData input, List<double> output);
        public event NeuralNetworkResultHandler ResultComputed;

        /// <summary>
        /// Constructor
        /// </summary>
        public NeuralNetwork()
        {
            Sensors = new List<Node>();
            Neurons = new List<List<ComputationalNode>>();
        }

        public NeuralNetwork(BinaryReader reader) : this()
        {
            Load(reader);
        }

        public bool Equals(NeuralNetwork other)
        {
            if (Sensors.Count != other.Sensors.Count)
            {
                return false;
            }

            // Check Sensors
            for (int i = 0; i < Sensors.Count; ++i)
            {
                if (!Sensors[i].Equals(other.Sensors[i]))
                {
                    return false;
                }
            }

            if (Neurons.Count != other.Neurons.Count)
            {
                return false;
            }

            // Check Each Layer
            for (int layer = 0; layer < Neurons.Count; ++layer)
            {
                if (Neurons[layer].Count != other.Neurons[layer].Count)
                {
                    return false;
                }

                for (int node = 0; node < Neurons[layer].Count; ++node)
                {
                    if (!Neurons[layer][node].Equals(other.Neurons[layer][node]))
                    {
                        return false;
                    }
                }
            }

            return true;
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

        public bool GetNodeByUUID(uint UUID, out Node node)
        {
            foreach (Node sensor in Sensors)
            {
                if (sensor.UUID == UUID)
                {
                    node = sensor;
                    return true;
                }
            }

            foreach (List<ComputationalNode> layer in Neurons)
            {
                foreach (ComputationalNode neuron in layer)
                {
                    if (neuron.UUID == UUID)
                    {
                        node = neuron;
                        return true;
                    }
                }
            }

            node = null;
            return false;
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

        protected virtual void Load(BinaryReader reader)
        {
            // Read the number of Sensors
            int numSensors = reader.ReadInt32();

            // Read the sensors
            for (int i = 0; i < numSensors; ++i)
            {
                Sensors.Add(new Node(reader));
            }

            // Read the number of layers
            int numLayers = reader.ReadInt32();

            // Read the Neuron Layers
            for (int layerIndex = 0; layerIndex < numLayers; ++layerIndex)
            {
                List<ComputationalNode> layer = new List<ComputationalNode>();

                // Read the number of Neurons in the layer
                int numNodes = reader.ReadInt32();
                for (int i = 0; i < numNodes; ++i)
                {
                    ComputationalNode node = new ComputationalNode(this, reader);
                    layer.Add(node);
                }

                Neurons.Add(layer);
            }
        }

        /// <summary>
        /// Vector of Neurons that belong to this Neural Net
        /// </summary>
        public List<List<ComputationalNode>> Neurons
        {
            get;
            set;
        }

        public void ProcessData(ScapeData scapeData)
        {
            List<double> input = scapeData.AsList();
            List<double> output = Compute(input);

            ResultComputed(scapeData, output);
        }

        public void Save(BinaryWriter writer)
        {
            // Write the number of Sensors
            writer.Write(Sensors.Count);

            // Write out the Sensors
            foreach (Node sensor in Sensors)
            {
                sensor.Save(writer);
            }

            // Write the number of layers
            writer.Write(Neurons.Count);
            
            // Write out the Neurons
            foreach (List<ComputationalNode> layer in Neurons)
            {
                // Write the number of Neurons in the layer
                writer.Write(layer.Count);
                foreach (ComputationalNode node in layer)
                {
                    node.Save(writer);
                }
            }
        }
        
        /// <summary>
        /// Vector of sensors that belong to this Neural Net
        /// </summary>
        public List<Node> Sensors
        {
            get;
            set;
        }
    }

}

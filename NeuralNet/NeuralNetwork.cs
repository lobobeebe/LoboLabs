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
        /// <param name="aggregationFunction"></param>
        /// <param name="errorFunction"></param>
        public NeuralNetwork()
        {
            Sensors = new List<Sensor>();
            Neurons = new List<Neuron>();
            Actuator = new Actuator();
        }    
        
        /// <summary>
        /// Final Actuator holding the output value
        /// </summary>
        public Actuator Actuator
        {
            get;
            set;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public double Compute(List<double> inputs)
        {
            if (inputs.Count != Sensors.Count)
            {
                Logger.Debug("Cannot Compute inputs of size " + inputs.Count + " with " + Sensors.Count + " Sensors.");
            }
            else
            {
                // Set input information of Sensors
                for (int i = 0; i < Sensors.Count; ++i)
                {
                    Sensors[i].SetLastOutput(inputs[i]);
                }
            }

            return Actuator.LastOutput;
        }

        /// <summary>
        /// Vector of Neurons that belong to this Neural Net
        /// </summary>
        public List<Neuron> Neurons
        {
            get;
            set;
        }

        public void ProcessData(List<double> data)
        {
            OnResult(Compute(data));
        }

        protected abstract void OnResult(double result);
        
        /// <summary>
        /// Vector of sensors that belong to this Neural Net
        /// </summary>
        public List<Sensor> Sensors
        {
            get;
            set;
        }
    }

}

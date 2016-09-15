using System.Collections.Generic;

using LoboLabs.NeuralNet.Messaging;
using LoboLabs.Utilities;

namespace LoboLabs.NeuralNet
{
    
    public abstract class ComputationalNode
    {
        private static ClassLogger Logger = new ClassLogger(typeof(ComputationalNode));

        public ComputationalNode(Functions.ActivationFunction activationFunction)
        {
            // Inputs
            Inputs = new List<ComputationalNode>();
            InputDataMap = new Dictionary<ComputationalNode, InputData>();

            // Outputs
            Outputs = new List<ComputationalNode>();
            OutputDataMap = new Dictionary<ComputationalNode, OutputData>();

            // Activation Function
            ActivationFunction = activationFunction;
        }

        /// <summary>
        /// Activation Function to apply when computing
        /// </summary>
        private Functions.ActivationFunction ActivationFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Propagates the error of this nodes last output to this nodes inputs
        /// TODO: Elaborate
        /// </summary>
        /// <param name="learningRate">TODO</param>
        public virtual void CalculateErrorSignals(double learningRate)
        {
            // Save the stored ErrorSignalSum locally for use.
            double errorSignalAggregation = ErrorSignalSum;

            // Reset the stored Error Sigal Sum for the next round. 
            ErrorSignalSum = 0;

            // Multiply by the derivative of the activation function once outside the loop of inputs
            errorSignalAggregation *= ActivationFunction.ApplyDerivative(LastOutput);

            // Compute Weight Gradients and Send Input Errors
            foreach (ComputationalNode input in Inputs)
            {
                // Calculate Weight Delta
                double weightDelta = learningRate * errorSignalAggregation * InputDataMap[input].Value;

                // Calculate and Send Input Errors
                double inputErrorSignal = errorSignalAggregation * InputDataMap[input].Weight;
                input.ProcessErrorSignal(inputErrorSignal);

                // Update Total Weight Delta to be updated at the end of the training
                InputDataMap[input].TotalWeightDelta += weightDelta;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        public virtual void Compute()
        {
            // Compute output based on input:
            // - Calculate the scalar product of the input vector and the weight vector
            // - Add a bias to the scalar product
            // - Finally, apply the activation function to the result

            // Apply the Dot Product on the Inputs and their Weight List
            double result = 0;
            foreach (ComputationalNode input in Inputs)
            {
                result += InputDataMap[input].Value * InputDataMap[input].Weight;
            }

            // Post-Processing
            LastOutput = ActivationFunction.Apply(result);
        }

        private double ErrorSignalSum
        {
            get;
            set;
        }

        /// <summary>
        /// A vector of NeuralNode's that are sending this node outputs.
        /// </summary>
        protected Dictionary<ComputationalNode, InputData> InputDataMap
        {
            get;
            private set;
        }

        protected List<ComputationalNode> Inputs
        {
            get;
            private set;
        }

        /// <summary>
        /// Stores the last output of this Neuron
        /// </summary>
        public double LastOutput
        {
            get;
            protected set;
        }

        /// <summary>
        /// A vector of the NeuralNodes to whcih this node will send messages.
        /// </summary>
        public Dictionary<ComputationalNode, OutputData> OutputDataMap
        {
            get;
            set;
        }

        protected List<ComputationalNode> Outputs
        {
            get;
            private set;
        }

        public void ProcessErrorSignal(double errorSignal)
        {
            ErrorSignalSum += errorSignal;
        }

        private void ProcessInput(ComputationalNode sender, double value)
        {
            // We just received new information, validate this data.
            InputDataMap[sender].Value = value; 
        }
        
        public void RegisterInput(ComputationalNode item, double inputWeight)
        {
            Inputs.Add(item);

            InputData inputData = new InputData();
            inputData.Weight = inputWeight;
            InputDataMap.Add(item, inputData);
        }
        
        public void RegisterOutput(ComputationalNode item)
        {
            Outputs.Add(item);

            OutputData outputData = new OutputData();
            OutputDataMap.Add(item, outputData);
        }

        public void UpdateWeightDeltas()
        {
            foreach (ComputationalNode input in Inputs)
            {
                InputDataMap[input].Weight += InputDataMap[input].TotalWeightDelta;
            }
        }
    }

}

using System.Collections.Generic;

using LoboLabs.Utilities;
using System.IO;

namespace LoboLabs.NeuralNet
{
    using System;
    using Functions;
    using System.Linq;

    public class ComputationalNode : Node
    {
        private static ClassLogger Logger = new ClassLogger(typeof(ComputationalNode));

        public ComputationalNode(BinaryReader reader) : this()
        {
            Load(reader);
        }

        public ComputationalNode() :
            this(new NoOpActivationFunction()) // Activation Function will default to No Op Function
        {
        }

        public ComputationalNode(ActivationFunction activationFunction) :
            base(true) // ComputationalNodes need to sum ErrorSignals at the Node level
        {
            // Inputs
            InputWeights = new Dictionary<Node, WeightData>();
                        
            ActivationFunction = activationFunction;
        }

        /// <summary>
        /// Activation Function to apply when computing
        /// </summary>
        private ActivationFunction ActivationFunction
        {
            get;
            set;
        }

        public double Bias
        {
            get;
            set;
        }

        /// <summary>
        /// Propagates the error of this nodes last output to this nodes inputs
        /// TODO: Elaborate
        /// </summary>
        /// <param name="learningRate">TODO</param>
        public virtual void CalculateErrorSignals()
        {
            // gⱼ = ActivationFunction
            // 𝛅ⱼ = gⱼ'(zⱼ)(Σwⱼₖ𝛅ₖ + e)
            // ∂E/∂wᵢⱼ = aᵢ𝛅ⱼ
            // ∂E/∂bⱼ = 𝛅ⱼ

            double delta = GetAndResetErrorSignalSum() * ActivationFunction.ApplyDerivative(LastPreActivationOutput);

            // Compute Weight Gradients and Send Input Errors
            foreach (Node input in InputWeights.Keys)
            {
                // Calculate Weight Delta
                InputWeights[input].TotalWeightDelta += input.LastOutput * delta;

                // Calculate Bias Delta
                TotalBiasDelta += delta;

                // Calculate and Send Input Error
                input.AddErrorSignal(InputWeights[input].Weight * delta);
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
            LastPreActivationOutput = 0;
            foreach (Node input in InputWeights.Keys)
            {
                LastPreActivationOutput += input.LastOutput * InputWeights[input].Weight;
            }

            // Add the bias
            LastPreActivationOutput += Bias;

            // Post-Processing
            LastOutput = ActivationFunction.Apply(LastPreActivationOutput);
        }

        public bool Equals(ComputationalNode other)
        {
            bool isEqual = false;

            // Base class, Activation Function, Bias, and Weights
            if (base.Equals(other) &&
                ActivationFunction.Equals(other.ActivationFunction) &&
                Bias == other.Bias &&
                InputWeights.Count == other.InputWeights.Count)
            {
                bool foundAll = true;

                // Must do a double for loop because Nodes arent comparable
                foreach (KeyValuePair<Node, WeightData> pair in InputWeights)
                {
                    bool found = false;
                    foreach (KeyValuePair<Node, WeightData> otherPair in InputWeights)
                    {
                        if (pair.Key.Equals(otherPair.Key) &&
                            pair.Value.Equals(otherPair.Value))
                        {
                            found = true;
                            break;
                        }
                    }

                    if(!found)
                    {
                        foundAll = false;
                        break;
                    }
                }

                isEqual = foundAll;
            }

            return isEqual;
        }

        public Dictionary<Node, WeightData> InputWeights
        {
            get;
            private set;
        }

        public double LastPreActivationOutput
        {
            get;
            set;
        }
        
        protected override void Load(BinaryReader reader)
        {
            // Read the base class
            base.Load(reader);

            // Read and create the Activation Function
            string functionName = reader.ReadString();
            ActivationFunction = FunctionCreator.CreateActivationFunction(functionName);

            // Read the Bias
            Bias = reader.ReadDouble();

            // Read the number of Inputs
            int numInputPairs = reader.ReadInt32();

            // Read each input pair
            for (int i = 0; i < numInputPairs; ++i)
            {
                int UUID = reader.ReadInt32();
                double weight = reader.ReadDouble();

                Node inputNode = GetNodeByUUID(UUID);
                if (inputNode != null)
                {
                    WeightData weightData = new WeightData();
                    weightData.Weight = weight;

                    InputWeights.Add(inputNode, weightData);
                }
                else
                {
                    Logger.Error("Loading failed because a UUID was read that is not valid. " +
                        "This may be caused by a recurrent network. Need to fix this to support.");
                }
            }
        }

        public void RegisterInput(Node item, double inputWeight)
        {
            WeightData inputData = new WeightData();
            inputData.Weight = inputWeight;
            InputWeights.Add(item, inputData);
        }

        private double TotalBiasDelta
        {
            get;
            set;
        }

        public void UpdateWeightDeltas(double learningRate)
        {
            foreach (Node input in InputWeights.Keys)
            {
                InputWeights[input].Weight -= learningRate * InputWeights[input].TotalWeightDelta;
                InputWeights[input].TotalWeightDelta = 0;
            }

            Bias -= learningRate * TotalBiasDelta;
            TotalBiasDelta = 0;

            // Revert values that are no longer relevant for this sequence
            GetAndResetErrorSignalSum();

            LastOutput = 0;
            LastPreActivationOutput = 0;
        }

        public override void Save(BinaryWriter writer)
        {
            // Save the base class
            base.Save(writer);

            // Save the name of the Activation Function
            writer.Write(ActivationFunction.GetFunctionName());

            // Save the Bias
            writer.Write(Bias);

            // Save Number of Inputs
            writer.Write(InputWeights.Count);

            // Write each input node's UUID and its weight
            foreach (KeyValuePair<Node, WeightData> pair in InputWeights)
            {
                writer.Write(pair.Key.UUID);
                writer.Write(pair.Value.Weight);
            }
        }
    }

}

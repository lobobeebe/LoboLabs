using System.Collections.Generic;

using LoboLabs.Utilities;

namespace LoboLabs.NeuralNet
{
    public class ComputationalNode : Node
    {
        private static ClassLogger Logger = new ClassLogger(typeof(ComputationalNode));

        public ComputationalNode(Functions.ActivationFunction activationFunction) :
            base(true) // ComputationalNodes need to sum ErrorSignals at the Node level
        {
            // Inputs
            InputWeights = new Dictionary<Node, WeightData>();
            
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

        public string Name
        {
            get;
            set;
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
    }

}

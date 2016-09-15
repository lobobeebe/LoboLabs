using System.Collections.Generic;
using LoboLabs.Utilities;

namespace LoboLabs.NeuralNet
{

    public abstract class NeuralNetworkTrainer<Input> : ScapeListener where Input : NetworkInputType
    {
        private static ClassLogger Logger = new ClassLogger(typeof(NeuralNetworkTrainer<Input>));

        private const int DEFAULT_MAX_EPOCHS = 100;
        private const double DEFAULT_LEARNING_RATE = 0.05; // TODO: This doesn't do anything.

        public NeuralNetworkTrainer(Functions.ErrorFunction errorFunction)
        {
            ErrorFunction = errorFunction;

            MaxEpochs = DEFAULT_MAX_EPOCHS;
            LearningRate = DEFAULT_LEARNING_RATE;
        }
        
        private double CalculateMeanError(List<TrainingData<Input>> trainData, NeuralNetwork network)
        {
            double error = 0.0;
            double sumError = 0.0;

            for (int i = 0; i < trainData.Count; ++i)
            {
                List<double> expectedOutputs = trainData[i].ExpectedOutputs;
                List<double> actualOutputs = new List<double>(expectedOutputs.Count); 

                for (int j = 0; j < trainData[i].InputValues.Count; ++j)
                {
                    actualOutputs.Add(network.Compute(trainData[i].InputValues[j].ToList()));
                }

                error = ErrorFunction.Error(expectedOutputs, actualOutputs);

                sumError += error;
            }

            return (trainData.Count > 0) ? (sumError / trainData.Count) : 0;
        }

        protected abstract List<TrainingData<Input>> CompileTrainingData();
        
        /// <summary>
        /// TODO
        /// </summary>
        private Functions.ErrorFunction ErrorFunction
        {
            get;
            set;
        }

        public double LearningRate
        {
            get;
            set;
        }

        public int MaxEpochs
        {
            get;
            set;
        }

        public abstract void ProcessData(List<double> data);

        public void TrainBackPropagation(NeuralNetwork network)
        {
            List<TrainingData<Input>> trainingData = CompileTrainingData();

            // Interval to print error
            int errInterval = MaxEpochs / 10;

            for (int epoch = 0; epoch < MaxEpochs; ++epoch)
            {
                // Print out Mean Error for debug purposes
                if (errInterval == 0 || epoch % errInterval == 0)
                {
                    double meanError = CalculateMeanError(trainingData, network);
                    Logger.Debug("Epoch " + epoch + ":  Error = " +
                        meanError);
                }

                // Each Training Datum has a list of inputs, ie. vectors, that will be interpreted as a series.
                for (int trainingDatumIndex = 0; trainingDatumIndex < trainingData.Count; ++trainingDatumIndex)
                {
                    TrainingData<Input> trainingDatum = trainingData[trainingDatumIndex];
                    List<Input> inputs = trainingDatum.InputValues;
                    List<double> expectedOutputs = trainingDatum.ExpectedOutputs;

                    // Each datum will be given to the network in order and the results will be saved
                    List<double> actualOutputs = new List<double>(expectedOutputs.Count);

                    for (int seriesIndex = 0; seriesIndex < inputs.Count; ++seriesIndex)
                    {
                        actualOutputs[seriesIndex] = network.Compute(inputs[seriesIndex].ToList());
                    }

                    // Starting from the last output, compute the error and propogate it back through the network
                    for (int seriesIndex = inputs.Count - 1; seriesIndex >= 0; --seriesIndex)
                    {
                        // Ei = ExpectedAi - ActualAi
                        double outputError = expectedOutputs[seriesIndex] - actualOutputs[seriesIndex];

                        // Calculate the Actuator's Error Signals since it is the last
                        network.Actuator.ProcessErrorSignal(outputError);
                        network.Actuator.CalculateErrorSignals(LearningRate);

                        // The actuator has updated all of the Error Signals of the hidden nodes
                        for (int neuronIndex = 0; neuronIndex < network.Neurons.Count; ++neuronIndex)
                        {
                            network.Neurons[neuronIndex].CalculateErrorSignals(LearningRate);
                        }
                    }

                    // Solidify Training Results by updating weights of each node with their deltas
                    network.Actuator.UpdateWeightDeltas();
                    
                    for (int neuronIndex = 0; neuronIndex < network.Neurons.Count; ++neuronIndex)
                    {
                        network.Neurons[neuronIndex].UpdateWeightDeltas();
                    }
                } 
            }
        }
    }

}
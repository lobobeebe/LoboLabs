using System.Collections.Generic;
using LoboLabs.Utilities;

namespace LoboLabs.NeuralNet
{

    public abstract class NeuralNetworkTrainer<Input> : ScapeListener where Input : NetworkInputType
    {
        private static ClassLogger Logger = new ClassLogger(typeof(NeuralNetworkTrainer<Input>));

        private const int DEFAULT_MAX_EPOCHS = 100;
        private const double DEFAULT_LEARNING_RATE = 1;

        public NeuralNetworkTrainer(Functions.ErrorFunction errorFunction)
        {
            ErrorFunction = errorFunction;

            MaxEpochs = DEFAULT_MAX_EPOCHS;
            LearningRate = DEFAULT_LEARNING_RATE;
        }
        
        protected double CalculateMeanError(List<TrainingData<Input>> trainData, NeuralNetwork network)
        {
            double error = 0.0;
            double sumError = 0.0;

            if (trainData.Count > 0)
            {
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

                sumError /= trainData.Count;
            }

            return sumError;
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
                double sumError = 0;
                
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
                        actualOutputs.Add(network.Compute(inputs[seriesIndex].ToList()));
                    }

                    double seriesSumError = 0;

                    // Starting from the last output, compute the error and propogate it back through the network
                    for (int seriesIndex = inputs.Count - 1; seriesIndex >= 0; --seriesIndex)
                    {
                        // Ei = ExpectedAi - ActualAi
                        double outputError = actualOutputs[seriesIndex] - expectedOutputs[seriesIndex];
                        seriesSumError += outputError;

                        // Artifically add the Actuator's Error Signals since it is the output
                        // TODO: Change when multiple actuators are allowed
                        network.GetActuator().AddErrorSignal(outputError);

                        // The actuator has updated all of the Error Signals of the hidden nodes
                        for (int layerIndex = network.Neurons.Count - 1; layerIndex >= 0; --layerIndex)
                        {
                            for (int nodeIndex = 0; nodeIndex < network.Neurons[layerIndex].Count; ++nodeIndex)
                            {
                                network.Neurons[layerIndex][nodeIndex].CalculateErrorSignals();
                                network.Neurons[layerIndex][nodeIndex].UpdateWeightDeltas(LearningRate);
                            }
                        }
                    }

                    if(inputs.Count > 0)
                    {
                        sumError += seriesSumError / inputs.Count;
                    }

                    // Solidify Training Results by updating weights of each node with their deltas
                    for (int layerIndex = 0; layerIndex < network.Neurons.Count; ++layerIndex)
                    {
                        for (int nodeIndex = 0; nodeIndex < network.Neurons[layerIndex].Count; ++nodeIndex)
                        {
                            //network.Neurons[layerIndex][nodeIndex].UpdateWeightDeltas(LearningRate);
                        }
                    }
                }

                if (trainingData.Count > 0)
                {
                    sumError /= trainingData.Count;
                }

                // Print out Mean Error for debug purposes
                if (errInterval == 0 || epoch % errInterval == 0)
                {
                    Logger.Debug("Epoch " + epoch + ":  Error = " +
                        sumError);
                }

                sumError = 0;
            }
        }
    }

}
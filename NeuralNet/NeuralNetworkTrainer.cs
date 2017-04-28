using System;
using System.Collections.Generic;
using LoboLabs.Utilities;

namespace LoboLabs.NeuralNet
{

    public class NeuralNetworkTrainer
    {
        private static ClassLogger Logger = new ClassLogger(typeof(NeuralNetworkTrainer));

        private const int DEFAULT_MAX_EPOCHS = 100;
        private const double DEFAULT_LEARNING_RATE = 1;

        private List<ScapeDataDefinition> mDefinitions;
        private int mNumInputs;

        public NeuralNetworkTrainer(Functions.ErrorFunction errorFunction, int numInputs)
        {
            ErrorFunction = errorFunction;
            mNumInputs = numInputs;

            mDefinitions = new List<ScapeDataDefinition>();
            MaxEpochs = DEFAULT_MAX_EPOCHS;
            LearningRate = DEFAULT_LEARNING_RATE;
        }
        
        protected double CalculateMeanError(NeuralNetwork network, List<TrainingData> trainingData)
        {
            double error = 0.0;
            double sumError = 0.0;

            if (trainingData.Count > 0)
            {
                for (int i = 0; i < trainingData.Count; ++i)
                {
                    List<double> expectedOutputs = trainingData[i].ExpectedOutputs;
                    List<double> actualOutputs = network.Compute(trainingData[i].InputValues);

                    error = ErrorFunction.Error(expectedOutputs, actualOutputs);

                    sumError += error;
                }

                sumError /= trainingData.Count;
            }

            return sumError;
        }

        public List<TrainingData> CreateTrainingDataFromDefinitions(List<ScapeDataDefinition> definitions)
        {
            List<TrainingData> trainingDataList = new List<TrainingData>();
            
            for (int i = 0; i < definitions.Count; ++i)
            {
                List<double> outputs = new List<double>(new double[definitions.Count]);

                // Set the current index to 1 indicating that it is the correct answer
                outputs[i] = 1.0;

                foreach(ScapeData data in definitions[i].DataList)
                {
                    trainingDataList.Add(new TrainingData(data.AsList(), outputs));
                }
            }

            return trainingDataList;
        }

        public string CurrentOutputName
        {
            get;
            set;
        }
        
        /// <summary>
        /// TODO
        /// </summary>
        private Functions.ErrorFunction ErrorFunction
        {
            get;
            set;
        }

        private ScapeDataDefinition GetDataDefinitionByName(string name)
        {
            ScapeDataDefinition returnDefinition = null;
            foreach (ScapeDataDefinition definition in mDefinitions)
            {
                if (definition.Name == name)
                {
                    returnDefinition = definition;
                }
            }

            // If not found, create a new one and add it to the Definitions list
            if (returnDefinition == null)
            {
                returnDefinition = new ScapeDataDefinition(name, mNumInputs);
                mDefinitions.Add(returnDefinition);
            }

            return returnDefinition;
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

        public void ProcessData(object sender, ScapeData scapeData)
        {
            // Find or create the current Data Definition
            ScapeDataDefinition currentDefinition = GetDataDefinitionByName(CurrentOutputName);

            currentDefinition.AddScapeData(scapeData);
        }

        public void TrainBackPropagation(NeuralNetwork network)
        {
            // Create the training data from ScapeDataDefintions
            List<TrainingData> trainingData = CreateTrainingDataFromDefinitions(mDefinitions);

            // Interval to print error
            int errInterval = MaxEpochs / 10;

            for (int epoch = 0; epoch < MaxEpochs; ++epoch)
            {
                // Print out Mean Error for debug purposes
                if (errInterval == 0 || epoch % errInterval == 0)
                {
                    Logger.Debug("Epoch " + epoch + ":  Error = " +
                        CalculateMeanError(network, trainingData));
                }
                
                for (int trainingDatumIndex = 0; trainingDatumIndex < trainingData.Count; ++trainingDatumIndex)
                {
                    TrainingData trainingDatum = trainingData[trainingDatumIndex];
                    List<double> inputs = trainingDatum.InputValues;
                    List<double> expectedOutputs = trainingDatum.ExpectedOutputs;

                    // Each datum will be given to the network in order and the results will be saved
                    List<double> actualOutputs = network.Compute(inputs);
                    
                    if(actualOutputs.Count != expectedOutputs.Count)
                    {
                        // Training a Network with non-matching training data
                        Logger.Error("Mismatch in number of training data outputs and network outputs. Exitting training.");
                        return;
                    }

                    // The actuator has updated all of the Error Signals of the hidden nodes
                    for (int layerIndex = network.Neurons.Count - 1; layerIndex >= 0; --layerIndex)
                    {
                        for (int nodeIndex = 0; nodeIndex < network.Neurons[layerIndex].Count; ++nodeIndex)
                        {
                            // The last layer is the actuator layer - add error signals.
                            if(layerIndex == network.Neurons.Count - 1)
                            {
                                double outputError = actualOutputs[nodeIndex] - expectedOutputs[nodeIndex];

                                network.Neurons[layerIndex][nodeIndex].AddErrorSignal(outputError);
                            }

                            network.Neurons[layerIndex][nodeIndex].CalculateErrorSignals();
                            network.Neurons[layerIndex][nodeIndex].UpdateWeightDeltas(LearningRate);
                        }
                    }
                }
            }
        }
    }

}
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

        private List<TrainingData> mTrainingData;
        private int mNumInputs;
        private List<string> mOutputNames;
        private string mCurrentOutputName;

        public NeuralNetworkTrainer(Functions.ErrorFunction errorFunction, int numInputs, List<string> ouputNames)
        {
            if(ouputNames == null || ouputNames.Count == 0)
            {
                throw new NotImplementedException("Output names must be populated.");
            }

            ErrorFunction = errorFunction;
            mNumInputs = numInputs;
            mOutputNames = ouputNames;
            mCurrentOutputName = mOutputNames[0];

            mTrainingData = new List<TrainingData>();
            MaxEpochs = DEFAULT_MAX_EPOCHS;
            LearningRate = DEFAULT_LEARNING_RATE;
        }
        
        protected double CalculateMeanError(NeuralNetwork network)
        {
            double error = 0.0;
            double sumError = 0.0;

            if (mTrainingData.Count > 0)
            {
                for (int i = 0; i < mTrainingData.Count; ++i)
                {
                    List<double> expectedOutputs = mTrainingData[i].ExpectedOutputs;
                    List<double> actualOutputs = network.Compute(mTrainingData[i].InputValues);

                    error = ErrorFunction.Error(expectedOutputs, actualOutputs);

                    sumError += error;
                }

                sumError /= mTrainingData.Count;
            }

            return sumError;
        }

        public string CurrentOutputName
        {
            get
            {
                return mCurrentOutputName;
            }
            set
            {
                if(mOutputNames.Contains(value))
                {
                    mCurrentOutputName = value;
                }
            }
        }
        
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

        public void ProcessData(object sender, List<double> data)
        {
            if(data.Count != mNumInputs)
            {
                throw new NotImplementedException("Mismatch number of inputs and the data count received.");
            }

            int nameIndex = mOutputNames.IndexOf(CurrentOutputName);
            if(nameIndex >= 0 && nameIndex < mOutputNames.Count)
            {
                List<double> outputs = new List<double>();
                for (int outputIndex = 0; outputIndex < mOutputNames.Count; ++outputIndex)
                {
                    if (outputIndex == nameIndex)
                    {
                        outputs.Add(1);
                    }
                    else
                    {
                        outputs.Add(0);
                    }
                }

                mTrainingData.Add(new TrainingData(data, outputs));
            }
            else
            {
                throw new NotImplementedException("Current output name, " + CurrentOutputName +
                    ", is not in the list of output names. Ignoring data.");
            }
        }

        public void TrainBackPropagation(NeuralNetwork network)
        {
            // Interval to print error
            int errInterval = MaxEpochs / 10;

            for (int epoch = 0; epoch < MaxEpochs; ++epoch)
            {
                // Print out Mean Error for debug purposes
                if (errInterval == 0 || epoch % errInterval == 0)
                {
                    Logger.Debug("Epoch " + epoch + ":  Error = " +
                        CalculateMeanError(network));
                }
                
                for (int trainingDatumIndex = 0; trainingDatumIndex < mTrainingData.Count; ++trainingDatumIndex)
                {
                    TrainingData trainingDatum = mTrainingData[trainingDatumIndex];
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
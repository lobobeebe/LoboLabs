﻿using System.Collections.Generic;
using LoboLabs.NeuralNet;
using LoboLabs.Utilities;
using LoboLabs.NeuralNet.Functions;

namespace LoboLabs.GestureNeuralNet
{
    
    public class BinaryGestureTrainer : NeuralNetworkTrainer<Vector>, GestureScapeListener
    {
        private static ClassLogger Logger = new ClassLogger(typeof(BinaryGestureTrainer));

        public BinaryGestureTrainer() : base(new SumSquaredError())
        {
            PositiveTrainingData = new List<List<Vector>>();
            NegativeTrainingData = new List<List<Vector>>();
        }

        protected override List<TrainingData<Vector>> CompileTrainingData()
        {
            // Initialize vector of training data
            List<TrainingData<Vector>> trainingData = new List<TrainingData<Vector>>(
                PositiveTrainingData.Count + NegativeTrainingData.Count);

            // Positive Training
            foreach (List<Vector> gesture in PositiveTrainingData)
            {
                // Create the expected output.
                // This should be the percentage of the way through a gesture
                List<double> expectedOutputs = new List<double>(gesture.Count);
                
                for (int positionIndex = 0; positionIndex < gesture.Count; ++positionIndex)
                {
                    expectedOutputs.Add(((float)positionIndex) / gesture.Count);
                }

                trainingData.Add(new TrainingData<Vector>(gesture, expectedOutputs));
            }

            // Negative Training
            foreach (List<Vector> gesture in NegativeTrainingData)
            {
                // Create the expected output.
                // Expected Output should always be a zero
                List<double> expectedOutputs = new List<double>(gesture.Count);

                for (int positionIndex = 0; positionIndex < gesture.Count; ++positionIndex)
                {
                    expectedOutputs.Add(0);
                }

                trainingData.Add(new TrainingData<Vector>(gesture, expectedOutputs));
            }
            
            return trainingData;
        }

        public bool IsPositiveTraining
        {
            get;
            set;
        }

        private List<List<Vector>> NegativeTrainingData
        {
            get;
            set;
        }

        public List<List<Vector>> PositiveTrainingData
        {
            get;
            private set;
        }

        public override void ProcessData(List<double> data)
        {
            if (IsPositiveTraining)
            {
                if (PositiveTrainingData.Count > 0)
                {
                    // Add a position to the end of the last gesture
                    PositiveTrainingData[PositiveTrainingData.Count - 1].Add(Vector.FromList(data));
                }
                else
                {
                    Logger.Error("Cannot add position of positive training gesture to empty list.");
                }
            }
            else
            {
                if (PositiveTrainingData.Count > 0)
                {
                    // Add a position to the end of the last gesture
                    NegativeTrainingData[NegativeTrainingData.Count - 1].Add(Vector.FromList(data));
                }
                else
                {
                    Logger.Error("Cannot add position of negative training gesture to empty list.");
                }
            }
        }

        public void ProcessStartGesturing()
        {
            // If the last gesture is not empty, add a new gesture (list of vectors)
            if (IsPositiveTraining)
            {
                PositiveTrainingData.Add(new List<Vector>());
            }
            else
            {
                NegativeTrainingData.Add(new List<Vector>());
            }
        }

        public void ProcessStopGesturing()
        {
            // Do Nothing
        }
    }

}
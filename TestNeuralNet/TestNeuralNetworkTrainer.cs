using System.Collections.Generic;
using LoboLabs.Utilities;

namespace LoboLabs.NeuralNet.TestNeuralNetwork
{
    using NUnit.Framework;

    [TestFixture]
    public class TestNeuralNetworkTrainer : NeuralNetworkTrainer<DoublesPair>
    {
        private static ClassLogger Logger = new ClassLogger(typeof(TestNeuralNetworkTrainer));

        private Mock.NeuralNetwork mNeuralNet;

        public TestNeuralNetworkTrainer() : base(new Functions.SumSquaredError())
        {

        }

        protected override List<TrainingData<DoublesPair>> CompileTrainingData()
        {
            List<TrainingData<DoublesPair>> trainingData = new List<TrainingData<DoublesPair>>();

            if(Mode == "1s")
            {
                const int NUM_DATA_SETS = 40;
                const int NUM_VALUES = 10;

                for(int setIndex = 0; setIndex < NUM_DATA_SETS; ++setIndex)
                {
                    List<DoublesPair> trainingInputs = new List<DoublesPair>(NUM_VALUES);
                    List<double> trainingOutputs = new List<double>(NUM_VALUES);
                    for(int valueIndex = 0; valueIndex < NUM_VALUES; ++valueIndex)
                    {
                        double value = System.Math.Round(MathUtils.NextRand());
                        trainingInputs.Add(new DoublesPair(value));

                        double output = 0;
                        if (value == 1)
                        {
                            output = 1.0 / NUM_VALUES;
                        }

                        trainingOutputs.Add(output);
                    }

                    double sum = 0;
                    for(int valueIndex = 0; valueIndex < NUM_VALUES; ++valueIndex)
                    {
                        trainingOutputs[valueIndex] += sum;
                        sum = trainingOutputs[valueIndex];
                    }

                    trainingData.Add(new TrainingData<DoublesPair>(trainingInputs, trainingOutputs));
                }
            }
            else if (Mode == "XOR")
            {
                const int NUM_TRUTH_TABLES = 10;
                for (int i = 0; i < NUM_TRUTH_TABLES; ++i)
                {
                    trainingData.Add(new TrainingData<DoublesPair>(
                        new List<DoublesPair>() { new DoublesPair(0, 0) }, // Input (0, 0)
                        new List<double>() { 0 }));
                    trainingData.Add(new TrainingData<DoublesPair>(
                        new List<DoublesPair>() { new DoublesPair(0, 1) }, // Input (0, 1)
                        new List<double>() { 1 }));
                    trainingData.Add(new TrainingData<DoublesPair>(
                        new List<DoublesPair>() { new DoublesPair(1, 0) }, // Input (1, 0)
                        new List<double>() { 1 }));
                    trainingData.Add(new TrainingData<DoublesPair>(
                        new List<DoublesPair>() { new DoublesPair(1, 1) }, // Input (1, 1)
                        new List<double>() { 0 }));
                }
            }
            else
            {
                Logger.Error("Mode of (" + Mode + ") not supported.");
            }
            return trainingData;
        }

        private string Mode
        {
            get;
            set;
        }

        public override void ProcessData(List<double> data)
        {
        }

        [SetUp]
        public void SetUp()
        {
            mNeuralNet = new Mock.NeuralNetwork();
        }

        private void TestValues(List<DoublesPair> inputs, List<double> expected)
        {
            Logger.Debug("Testing the inputs: ");
            string inputString = "{";
            string outputString = "{";
            string expectedString = "{";
            for(int i = 0; i < inputs.Count; ++i)
            {
                if (i > 0)
                {
                    inputString += ", ";
                    outputString += ", ";
                    expectedString += ", ";
                }

                // Inputs
                for (int j = 0; j < inputs[i].Count; ++j)
                {
                    inputString += inputs[i][j];
                }

                // Outputs
                outputString += System.Math.Round(mNeuralNet.Compute(inputs[i]), 2);

                // Expected
                expectedString += expected[i];
            }
            inputString += "}";
            outputString += "}";
            expectedString += "}";
            Logger.Debug("Inputs:  " + inputString);
            Logger.Debug("Results: " + outputString);
            Logger.Debug("Expected: " + expectedString);
        }

        [Test]
        public void Train1s()
        {
            // This tests simple recurrent functionality
            Mode = "1s";
            LearningRate = .04;

            // Add 1 sensor
            mNeuralNet.Sensors.Add(new Node());

            // Use a single node
            mNeuralNet.Neurons.Add(new List<ComputationalNode>());
            mNeuralNet.Neurons[0].Add(new ComputationalNode(new Functions.HyperbolicTangent()));
            mNeuralNet.Neurons[0][0].Bias = MathUtils.NextRand();

            // Register sensor as nodes input
            mNeuralNet.Neurons[0][0].RegisterInput(mNeuralNet.Sensors[0], MathUtils.NextRand());

            // Register self as input (RECURRENCY)
            mNeuralNet.Neurons[0][0].RegisterInput(mNeuralNet.Neurons[0][0], MathUtils.NextRand());

            // Traing the NeuralNet on the data from the Trainer
            TrainBackPropagation(mNeuralNet);

            // Test some sample data
            List<DoublesPair> input = new List<DoublesPair>();
            List<double> expected = new List<double>();

            input.Add(new DoublesPair(0));
            expected.Add(0);
            input.Add(new DoublesPair(1));
            expected.Add(.1);
            input.Add(new DoublesPair(0));
            expected.Add(.1);
            input.Add(new DoublesPair(1));
            expected.Add(.2);
            input.Add(new DoublesPair(1));
            expected.Add(.3);
            input.Add(new DoublesPair(0));
            expected.Add(.3);
            input.Add(new DoublesPair(1));
            expected.Add(.4);
            input.Add(new DoublesPair(0));
            expected.Add(.4);
            input.Add(new DoublesPair(1));
            expected.Add(.5);
            input.Add(new DoublesPair(1));
            expected.Add(.6);

            // Should be 6
            TestValues(input, expected);
        }

        [Test]
        public void TrainXOR()
        {
            Mode = "XOR";
            LearningRate = 1;

            // Add two sensors
            mNeuralNet.Sensors.Add(new Node());
            mNeuralNet.Sensors.Add(new Node());

            // Create a network with a 3, 1 topology
            mNeuralNet.Neurons.Add(new List<ComputationalNode>());
            mNeuralNet.Neurons[0].Add(new ComputationalNode(new Functions.LogisticFunction()));
            mNeuralNet.Neurons[0][0].Bias = MathUtils.NextRand();
            mNeuralNet.Neurons[0].Add(new ComputationalNode(new Functions.LogisticFunction()));
            mNeuralNet.Neurons[0][1].Bias = MathUtils.NextRand();
            mNeuralNet.Neurons[0].Add(new ComputationalNode(new Functions.LogisticFunction()));
            mNeuralNet.Neurons[0][2].Bias = MathUtils.NextRand();

            mNeuralNet.Neurons.Add(new List<ComputationalNode>());
            mNeuralNet.Neurons[1].Add(new ComputationalNode(new Functions.LogisticFunction()));
            mNeuralNet.Neurons[1][0].Bias = MathUtils.NextRand();

            // Add the sensors as inputs to the neurons in first layer
            mNeuralNet.Neurons[0][0].RegisterInput(mNeuralNet.Sensors[0], MathUtils.NextRand());
            mNeuralNet.Neurons[0][0].RegisterInput(mNeuralNet.Sensors[1], MathUtils.NextRand());
            mNeuralNet.Neurons[0][1].RegisterInput(mNeuralNet.Sensors[0], MathUtils.NextRand());
            mNeuralNet.Neurons[0][1].RegisterInput(mNeuralNet.Sensors[1], MathUtils.NextRand());
            mNeuralNet.Neurons[0][2].RegisterInput(mNeuralNet.Sensors[0], MathUtils.NextRand());
            mNeuralNet.Neurons[0][2].RegisterInput(mNeuralNet.Sensors[1], MathUtils.NextRand());

            // Add the first layer neurons as inputs to the second layer neuron
            mNeuralNet.Neurons[1][0].RegisterInput(mNeuralNet.Neurons[0][0], MathUtils.NextRand());
            mNeuralNet.Neurons[1][0].RegisterInput(mNeuralNet.Neurons[0][1], MathUtils.NextRand());
            mNeuralNet.Neurons[1][0].RegisterInput(mNeuralNet.Neurons[0][2], MathUtils.NextRand());

            // Calculate current Mean Error before training
            double initialError = CalculateMeanError(CompileTrainingData(), mNeuralNet);
            Logger.Debug("Initial Error: " + initialError);

            // Traing the NeuralNet on the data from the Trainer (Mock Trainer is setup as XOR Trainer)
            TrainBackPropagation(mNeuralNet);

            // Calculate Mean Error after training
            double finalError = CalculateMeanError(CompileTrainingData(), mNeuralNet);

            Logger.Debug("Final Error: " + finalError);
            Assert.True(finalError < initialError);
        }
    }
}

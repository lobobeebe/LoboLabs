using System.Collections.Generic;

namespace LoboLabs.GestureNeuralNet.Test
{
    using Utilities;
    using NUnit.Framework;
    using NeuralNet;
    using NeuralNet.Functions;

    [TestFixture]
    public class TestGestureNeuralNetwork
    {
        private GestureScape mScape;
        private NeuralNetworkTrainer mTrainer;
        private NeuralNetworkGenerator mGenerator;

        private Queue<string> mDetectedGestureQueue;

        private const string PUNCH_NAME = "Punch";
        private const string SWIPE_NAME = "Swipe";
        private const string CIRCLE_NAME = "Circle";
        private const int NUM_INPUT_VECTORS = 10;
        private const int NUM_VALUES_PER_VECTOR = 3;
        private const int NUM_OUTPUTS = 3;

        [SetUp]
        public void SetUp()
        {
            mScape = new GestureScape(NUM_INPUT_VECTORS);
            mTrainer = new NeuralNetworkTrainer(new SumSquaredError(), NUM_INPUT_VECTORS);
            mGenerator = new NeuralNetworkGenerator();

            mDetectedGestureQueue = new Queue<string>();

            // Add the trainer as a listener to the scape
            mScape.DataReceived += mTrainer.ProcessData;
        }

        public void AddStraightData(int numPoints)
        {
            mScape.StartGesturing();
            mScape.UpdateGesturePosition(new Vector(0, 0, 0));

            const float VARIANCE = .3f;
            float currentPosition = 0;

            for (int i = 0; i < numPoints - 1; ++i)
            {
                currentPosition += (float)MathUtils.NextRand(1 - VARIANCE, 1 + VARIANCE);
                mScape.UpdateGesturePosition(new Vector(0, 0, currentPosition));
            }

            mScape.StopGesturing();
        }

        public void AddSwipeData(int numPoints)
        {
            mScape.StartGesturing();
            mScape.UpdateGesturePosition(new Vector(0, 0, 0));

            const float VARIANCE = .3f;
            float currentPosition = 0;

            for (int i = 0; i < numPoints - 1; ++i)
            {
                currentPosition += (float)MathUtils.NextRand(1 - VARIANCE, 1 + VARIANCE);
                mScape.UpdateGesturePosition(new Vector(0, currentPosition, 0));
            }

            mScape.StopGesturing();
        }

        public void AddCircleData(float radius, int numPoints)
        {
            mScape.StartGesturing();
            mScape.UpdateGesturePosition(new Vector(0, 0, 0));

            for (double angle = 0; angle < 2 * System.Math.PI;
                angle += 2 * System.Math.PI / (numPoints - 1))
            {
                Vector position = new Vector((float)System.Math.Cos(angle) * radius, (float)System.Math.Sin(angle) * radius, 0);
                mScape.UpdateGesturePosition(position);
            }

            mScape.StopGesturing();
        }

        public void AddRandomData(float radius, int numPoints)
        {
            mScape.StartGesturing();
            mScape.UpdateGesturePosition(new Vector(0, 0, 0));

            for (int i = 0; i < numPoints - 1; ++i)
            {
                float randomAngle = (float)MathUtils.NextRand(0, 2 * System.Math.PI);
                mScape.UpdateGesturePosition(new Vector((float)System.Math.Cos(randomAngle) * radius,
                    (float)System.Math.Sin(randomAngle) * radius, (float)System.Math.Sin(randomAngle) * radius));
            }

            mScape.StopGesturing();
        }

        [Test]
        public void SingleGesture()
        {
            // Teach Circle Gestures
            mTrainer.CurrentOutputName = "Circle";
            const int NUM_CIRCLE_GESTURES = 30;
            for (int i = 0; i < NUM_CIRCLE_GESTURES; ++i)
            {
                AddCircleData((float)MathUtils.NextRand(.95, 1.05), (int)MathUtils.NextRand(25, 35));
            }

            // Teach Punch Gestures
            mTrainer.CurrentOutputName = "Punch";
            const int NUM_PUNCH_GESTURES = 30;
            for (int i = 0; i < NUM_PUNCH_GESTURES; ++i)
            {
                AddStraightData((int)MathUtils.NextRand(25, 35));
            }

            // Teach Swipe Gestures
            mTrainer.CurrentOutputName = "Swipe";
            const int NUM_SWIPE_GESTURES = 30;
            for (int i = 0; i < NUM_SWIPE_GESTURES; ++i)
            {
                AddSwipeData((int)MathUtils.NextRand(25, 35));
            }

            // Save the Training Data to a file
            //GestureIOManager.SaveGesturesToPath("TestGestures", mTrainer.DefinitionsList);

            // Load the Training Data from a file
            //List<ScapeDataDefinition> definitionList = GestureIOManager.GetGesturesFromPath("TestGestures");
            //List<string> definitionNameList = ScapeDataDefinition.GetNamesFromList(definitionList);

            // Generate a Gesture NeuralNet with three hidden nodes
            mGenerator.NumHidden = 3;
            NeuralNetwork networkMultiple = mGenerator.Generate(
                NUM_INPUT_VECTORS * NUM_VALUES_PER_VECTOR,
                ScapeDataDefinition.GetNamesFromList(mTrainer.DefinitionsList));
            networkMultiple.ValidResultReceived += ProcessResult;

            // Train networks
            mTrainer.LearningRate = .2;
            mTrainer.TrainBackPropagation(networkMultiple, mTrainer.DefinitionsList);

            // Remove Trainer as listener and add Single hidden node NeuralNet as listener
            mScape.DataReceived -= mTrainer.ProcessData;
            mScape.DataReceived += networkMultiple.ProcessData;

            // Pass several Punch gestures to test the single hidden node network. It should recognize each one
            for (int i = 0; i < NUM_PUNCH_GESTURES; ++i)
            {
                AddStraightData(30);

                string lastGestureDetected;
                Assert.True(GetLastGestureDetected(out lastGestureDetected));
                Assert.AreEqual("Punch", lastGestureDetected);
            }

            // Pass several Circle gestures to test the single hidden node network. It should recognize each one
            for (int i = 0; i < NUM_CIRCLE_GESTURES; ++i)
            {
                AddCircleData(1, 30);

                string lastGestureDetected;
                Assert.True(GetLastGestureDetected(out lastGestureDetected));
                Assert.AreEqual("Circle", lastGestureDetected);
            }

            // Pass several Swipe gestures to test the single hidden node network. It should recognize each one
            for (int i = 0; i < NUM_SWIPE_GESTURES; ++i)
            {
                AddSwipeData(30);

                string lastGestureDetected;
                Assert.True(GetLastGestureDetected(out lastGestureDetected));
                Assert.AreEqual("Swipe", lastGestureDetected);
            }
        }

        public void ProcessResult(object sender, ScapeData input, string outputName, List<double> output)
        {
            mDetectedGestureQueue.Enqueue(outputName);
        }

        public bool GetLastGestureDetected(out string lastGestureName)
        {
            if (mDetectedGestureQueue.Count > 0)
            {
                lastGestureName = mDetectedGestureQueue.Dequeue();
                return true;
            }

            lastGestureName = "";
            return false;
        }
    }
}
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
        private GestureManager mGestureManager;
        private Queue<string> mDetectedGestureQueue;

        private const string PUNCH_NAME = "Punch";
        private const string SWIPE_NAME = "Swipe";
        private const string CIRCLE_NAME = "Circle";
        private const int NUM_INPUT_VECTORS = 10;
        private const int NUM_VALUES_PER_VECTOR = 3;
        private const int NUM_OUTPUTS = 3;
        private const double GESTURE_THRESHOLD = 0.75;

        [SetUp]
        public void SetUp()
        {
            mGestureManager = new GestureManager();
            mGestureManager.OnGestureDetected += OnGestureDetected;
            mDetectedGestureQueue = new Queue<string>();
        }

        public void AddStraightData(int numPoints)
        {
            mGestureManager.StartGesturing();
            mGestureManager.UpdateGesturePosition(new Vector(0, 0, 0));

            const float VARIANCE = .3f;
            float currentPosition = 0;

            for (int i = 0; i < numPoints - 1; ++i)
            {
                currentPosition += (float)MathUtils.NextRand(1 - VARIANCE, 1 + VARIANCE);
                mGestureManager.UpdateGesturePosition(new Vector(0, 0, currentPosition));
            }

            mGestureManager.StopGesturing();
        }

        public void AddSwipeData(int numPoints)
        {
            mGestureManager.StartGesturing();
            mGestureManager.UpdateGesturePosition(new Vector(0, 0, 0));

            const float VARIANCE = .3f;
            float currentPosition = 0;

            for (int i = 0; i < numPoints - 1; ++i)
            {
                currentPosition += (float)MathUtils.NextRand(1 - VARIANCE, 1 + VARIANCE);
                mGestureManager.UpdateGesturePosition(new Vector(0, currentPosition, 0));
            }

            mGestureManager.StopGesturing();
        }

        public void AddCircleData(float radius, int numPoints)
        {
            mGestureManager.StartGesturing();
            mGestureManager.UpdateGesturePosition(new Vector(0, 0, 0));

            for (double angle = 0; angle < 2 * System.Math.PI;
                angle += 2 * System.Math.PI / (numPoints - 1))
            {
                Vector position = new Vector((float)System.Math.Cos(angle) * radius, (float)System.Math.Sin(angle) * radius, 0);
                mGestureManager.UpdateGesturePosition(position);
            }

            mGestureManager.StopGesturing();
        }

        public void AddRandomData(float radius, int numPoints)
        {
            mGestureManager.StartGesturing();
            mGestureManager.UpdateGesturePosition(new Vector(0, 0, 0));

            for (int i = 0; i < numPoints - 1; ++i)
            {
                float randomAngle = (float)MathUtils.NextRand(0, 2 * System.Math.PI);
                mGestureManager.UpdateGesturePosition(new Vector((float)System.Math.Cos(randomAngle) * radius,
                    (float)System.Math.Sin(randomAngle) * radius, (float)System.Math.Sin(randomAngle) * radius));
            }

            mGestureManager.StopGesturing();
        }

        [Test]
        public void SingleGesture()
        {
            // Teach Punch Gestures
            mGestureManager.CurrentGestureName = "Punch";
            const int NUM_PUNCH_GESTURES = 30;
            for (int i = 0; i < NUM_PUNCH_GESTURES; ++i)
            {
                AddStraightData((int)MathUtils.NextRand(25, 35));
            }

            // Teach Circle Gestures
            mGestureManager.CurrentGestureName = "Circle";
            const int NUM_CIRCLE_GESTURES = 30;
            for (int i = 0; i < NUM_CIRCLE_GESTURES; ++i)
            {
                AddCircleData((float)MathUtils.NextRand(.95, 1.05), (int)MathUtils.NextRand(25, 35));
            }

            // Teach Swipe Gestures
            mGestureManager.CurrentGestureName = "Swipe";
            const int NUM_SWIPE_GESTURES = 30;
            for (int i = 0; i < NUM_SWIPE_GESTURES; ++i)
            {
                AddSwipeData((int)MathUtils.NextRand(25, 35));
            }

            // Save the Training Data to a file
            string gestureDirectory = "data/TestGestures";
            mGestureManager.SaveGesturesToPath(gestureDirectory);

            // Load the Training Data from a file
            mGestureManager.LoadAndTrainNetwork(gestureDirectory);

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

        public void OnGestureDetected(object sender, string gestureName)
        {
            mDetectedGestureQueue.Enqueue(gestureName);
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
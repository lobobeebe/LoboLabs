using LoboLabs.Utilities;
using NUnit.Framework;
using System.Collections.Generic;

namespace LoboLabs.GestureNeuralNet.Test
{
    using NeuralNet;
    using System.IO;
    using Utilities;

    [TestFixture]
    public class TestGestureDetector
    {
        private const string PUNCH_NAME = "Punch";
        private const int NUM_PUNCH_GESTURES = 30;

        private const string SWIPE_NAME = "Swipe";
        private const int NUM_SWIPE_GESTURES = 30;

        private const string CIRCLE_NAME = "Circle";
        private const int NUM_CIRCLE_GESTURES = 30;

        private Queue<string> mDetectedGestureQueue;
        private GestureNeuralNetworkGenerator mGenerator;
        private GestureDetector mDetector;

        [SetUp]
        public void SetUp()
        {
            mGenerator = new GestureNeuralNetworkGenerator();
            TrainDetector();

            mDetectedGestureQueue = new Queue<string>();
        }
        
        private void AddData(GestureDataReceiver receiver, List<Vector> gestureData)
        {
            receiver.StartGesturing();

            foreach (Vector position in gestureData)
            {
                receiver.UpdateGesturePosition(position);
            }

            receiver.StopGesturing();
        }

        private void TrainDetector()
        {
            // Teach Punch Gestures
            mGenerator.CurrentGestureName = PUNCH_NAME;
            for (int i = 0; i < NUM_PUNCH_GESTURES; ++i)
            {
                AddData(mGenerator,
                    GestureDataGenerator.CreateStraightData((int)MathUtils.NextRand(55, 65)));
            }

            // Teach Circle Gestures
            mGenerator.CurrentGestureName = CIRCLE_NAME;
            for (int i = 0; i < NUM_CIRCLE_GESTURES; ++i)
            {
                AddData(mGenerator,
                    GestureDataGenerator.CreateCircleData((float)MathUtils.NextRand(.95, 1.05),
                    (int)MathUtils.NextRand(55, 65)));
            }

            // Teach Swipe Gestures
            mGenerator.CurrentGestureName = SWIPE_NAME;
            for (int i = 0; i < NUM_SWIPE_GESTURES; ++i)
            {
                AddData(mGenerator,
                    GestureDataGenerator.CreateSwipeData((int)MathUtils.NextRand(55, 65)));
            }

            // Generate the Network and create the Detector
            NeuralNetwork network = mGenerator.Generate();
            mDetector = new GestureDetector(network, mGenerator.GetGestureNames());
            mDetector.OnGestureDetected += OnGestureDetected;
        }

        private void TestGestures()
        {
            // Pass several Punch gestures to test the single hidden node network. It should recognize each one
            for (int i = 0; i < NUM_PUNCH_GESTURES; ++i)
            {
                AddData(mDetector,
                    GestureDataGenerator.CreateStraightData(55));

                string lastGestureDetected;
                Assert.True(GetLastGestureDetected(out lastGestureDetected));
                Assert.AreEqual(PUNCH_NAME, lastGestureDetected);
            }

            // Pass several Circle gestures to test the single hidden node network. It should recognize each one
            for (int i = 0; i < NUM_CIRCLE_GESTURES; ++i)
            {
                AddData(mDetector,
                    GestureDataGenerator.CreateCircleData(1, 55));

                string lastGestureDetected;
                Assert.True(GetLastGestureDetected(out lastGestureDetected));
                Assert.AreEqual(CIRCLE_NAME, lastGestureDetected);
            }

            // Pass several Swipe gestures to test the single hidden node network. It should recognize each one
            for (int i = 0; i < NUM_SWIPE_GESTURES; ++i)
            {
                AddData(mDetector,
                    GestureDataGenerator.CreateSwipeData(55));

                string lastGestureDetected;
                Assert.True(GetLastGestureDetected(out lastGestureDetected));
                Assert.AreEqual(SWIPE_NAME, lastGestureDetected);
            }

            // Pass several Circles of the opposite direction
            for (int i = 0; i < NUM_CIRCLE_GESTURES; ++i)
            {
                AddData(mDetector,
                    GestureDataGenerator.CreateCircleData(1, 55, false));

                string lastGestureDetected;
                Assert.False(GetLastGestureDetected(out lastGestureDetected));
            }

            // Pass several random sets of data
            for (int i = 0; i < NUM_CIRCLE_GESTURES; ++i)
            {
                AddData(mDetector,
                    GestureDataGenerator.CreateRandomData(1, 55));

                string lastGestureDetected;
                Assert.False(GetLastGestureDetected(out lastGestureDetected));
            }
        }

        [Test]
        public void Equals()
        {
            // Test True case
            GestureDetector other = new GestureDetector(mDetector.GetNetwork(),
                mDetector.GetGestureNames());
            other.MinThreshold = mDetector.MinThreshold;

            Assert.True(mDetector.Equals(other));

            // Test MinThreshold different
            other.MinThreshold = 0;
            Assert.False(mDetector.Equals(other));
            other.MinThreshold = mDetector.MinThreshold;

            // TODO: Beef this up
        }

        [Test]
        public void SaveAndLoad()
        {
            // Generate the Network and create the Detector
            NeuralNetwork network = mGenerator.Generate();
            mDetector = new GestureDetector(network, mGenerator.GetGestureNames());
            mDetector.OnGestureDetected += OnGestureDetected;

            using (MemoryStream stream = new MemoryStream())
            {
                // Save the Detector to a file
                BinaryWriter writer = new BinaryWriter(stream);
                mDetector.Save(writer);

                // Reset Stream
                stream.Position = 0;

                // Read the Detector back in
                BinaryReader reader = new BinaryReader(stream);
                GestureDetector loadedDetector = new GestureDetector(reader);
                Assert.IsTrue(mDetector.Equals(loadedDetector));

                mDetector.OnGestureDetected += OnGestureDetected;
            }

            // Test that a detector loaded from a stream can detect gestures
            TestGestures();
        }

        [Test]
        public void SingleGesture()
        {
            // Test that the generated detector can detect gestures
            TestGestures();
        }
        
        private bool GetLastGestureDetected(out string lastGestureName)
        {
            if (mDetectedGestureQueue.Count > 0)
            {
                lastGestureName = mDetectedGestureQueue.Dequeue();
                return true;
            }

            lastGestureName = "";
            return false;
        }
        
        private void OnGestureDetected(string gestureName)
        {
            mDetectedGestureQueue.Enqueue(gestureName);
        }
    }
}
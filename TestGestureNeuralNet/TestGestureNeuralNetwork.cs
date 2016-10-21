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
        GestureDetectionManager mDetectionManager;

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
            mDetectionManager = new GestureDetectionManager();
            mDetectedGestureQueue = new Queue<string>();
        }
        
        public void AddStraightData(int numPoints)
        {
            mDetectionManager.StartRightHandGesturing();
            mDetectionManager.UpdateRightHandLocation(new Vector(0, 0, 0));

            const float VARIANCE = .3f;
            float currentPosition = 0;

            for (int i = 0; i < numPoints - 1; ++i)
            {
                currentPosition += (float)MathUtils.NextRand(1 - VARIANCE, 1 + VARIANCE);
                mDetectionManager.UpdateRightHandLocation(new Vector(0, 0, currentPosition));
            }

            mDetectionManager.StopRightHandGesturing();
        }

        public void AddSwipeData(int numPoints)
        {
            mDetectionManager.StartRightHandGesturing();
            mDetectionManager.UpdateRightHandLocation(new Vector(0, 0, 0));

            const float VARIANCE = .3f;
            float currentPosition = 0;

            for (int i = 0; i < numPoints - 1; ++i)
            {
                currentPosition += (float)MathUtils.NextRand(1 - VARIANCE, 1 + VARIANCE);
                mDetectionManager.UpdateRightHandLocation(new Vector(0, currentPosition, 0));
            }

            mDetectionManager.StopRightHandGesturing();
        }

        public void AddCircleData(float radius, int numPoints)
        {
            mDetectionManager.StartRightHandGesturing();
            mDetectionManager.UpdateRightHandLocation(new Vector(0, 0, 0));

            for (double angle = 0; angle < 2 * System.Math.PI;
                angle += 2 * System.Math.PI / (numPoints - 1))
            {
                Vector position = new Vector((float)System.Math.Cos(angle) * radius, (float)System.Math.Sin(angle) * radius, 0);
                mDetectionManager.UpdateRightHandLocation(position);
            }

            mDetectionManager.StopRightHandGesturing();
        }

        public void AddRandomData(float radius, int numPoints)
        {
            mDetectionManager.StartRightHandGesturing();
            mDetectionManager.UpdateRightHandLocation(new Vector(0, 0, 0));

            for (int i = 0; i < numPoints - 1; ++i)
            {
                float randomAngle = (float)MathUtils.NextRand(0, 2 * System.Math.PI);
                mDetectionManager.UpdateRightHandLocation(new Vector((float)System.Math.Cos(randomAngle) * radius, 
                    (float)System.Math.Sin(randomAngle) * radius, (float)System.Math.Sin(randomAngle) * radius));
            }

            mDetectionManager.StopRightHandGesturing();
        }
        
        [Test]
        public void SingleGesture()
        {
            // Teach Circle Gestures
            mDetectionManager.StartTrainingRightHandGestureName("Circle");
            const int NUM_CIRCLE_GESTURES = 30;
            for (int i = 0; i < NUM_CIRCLE_GESTURES; ++i)
            {
                AddCircleData((float)MathUtils.NextRand(.95, 1.05), (int)MathUtils.NextRand(25, 35));
            }

            // Teach Swipe Gestures
            mDetectionManager.StartTrainingRightHandGestureName("Swipe");
            const int NUM_SWIPE_GESTURES = 30;
            for (int i = 0; i < NUM_SWIPE_GESTURES; ++i)
            {
                AddSwipeData((int)MathUtils.NextRand(25, 35));
            }

            // Teach Punch Gestures
            mDetectionManager.StartTrainingRightHandGestureName("Punch");
            const int NUM_PUNCH_GESTURES = 30;
            for (int i = 0; i < NUM_PUNCH_GESTURES; ++i)
            {
                AddStraightData((int)MathUtils.NextRand(25, 35));
            }

            // Finalize the DetectionManager's training
            mDetectionManager.FinalizeTraining();
            mDetectionManager.RightHandGestureDetected += OnGestureDetected;
            
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

        public void OnGestureDetected(object sender, GestureDetectionEvent eventArgs)
        {
            mDetectedGestureQueue.Enqueue(eventArgs.Name);
        }

        public bool GetLastGestureDetected(out string lastGestureName)
        {
            if(mDetectedGestureQueue.Count > 0)
            {
                lastGestureName = mDetectedGestureQueue.Dequeue();
                return true;
            }

            lastGestureName = "";
            return false;
        }
    }

}

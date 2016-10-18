using System.Collections.Generic;

namespace LoboLabs.GestureNeuralNet.TestGestureNeuralNetwork
{
    using Utilities;
    using NUnit.Framework;

    [TestFixture]
    public class TestGestureNeuralNetwork : GestureDetectionListener
    {
        private GestureScape mScape;
        private BinaryGestureTrainer mTrainer;
        private BinaryGestureNeuralNetworkGenerator mGenerator;

        private Queue<string> mDetectedGestureQueue;

        [SetUp]
        public void SetUp()
        {
            mScape = new GestureScape();
            mTrainer = new BinaryGestureTrainer();
            mGenerator = new BinaryGestureNeuralNetworkGenerator();

            mDetectedGestureQueue = new Queue<string>();
            
            mScape.RegisterListener(mTrainer);
        }
        
        public void AddStraightData(int numPoints)
        {
            mScape.StartGesturing();
            mScape.UpdateGesturePosition(new Vector(0, 0, 0));

            const float VARIANCE = .3f;
            float currentPosition = 0;

            for (int i = 0; i < numPoints - 1; ++i)
            {
                mScape.UpdateGesturePosition(new Vector(0, 0, currentPosition + (float)MathUtils.NextRand(1 - VARIANCE, 1 + VARIANCE)));
            }

            mScape.StopGesturing();
        }

        public void AddCircleData(float radius, int numPoints)
        {
            mScape.StartGesturing();
            mScape.UpdateGesturePosition(new Vector(0, 0, 0));

            const float VARIANCE = .3f;
            //Vector lastPosition = new Vector(0, 0, 0);

            for (double angle = 0; angle < 2 * System.Math.PI; angle += 2 * System.Math.PI / (numPoints - 1))
            {
                float tempRadius = (float)MathUtils.NextRand(radius - VARIANCE, radius + VARIANCE);
                Vector position = new Vector((float)System.Math.Cos(angle) * tempRadius, (float)System.Math.Sin(angle) * tempRadius, (float)MathUtils.NextRand(-.1, .1));
                mScape.UpdateGesturePosition(position);// - lastPosition);
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
            /* Want to teach this neural net a single gesture by training on positive and negative gestures. The gesture to be taught will be a punch. */

            // Teach Positive Punch Gestures
            mTrainer.IsPositiveTraining = true;
            const int NUM_PUNCH_GESTURES = 30;
            for (int i = 0; i < NUM_PUNCH_GESTURES; ++i)
            {
                AddStraightData((int)MathUtils.NextRand(25, 35));
            }

            // Teach Negative Circle Gestures
            mTrainer.IsPositiveTraining = false;
            const int NUM_CIRCLE_GESTURES = 30;
            for(int i = 0; i < NUM_CIRCLE_GESTURES; ++i)
            {
                AddCircleData((float)MathUtils.NextRand(.8, 1.2), (int)MathUtils.NextRand(25, 35));
            }

            // Teach Negative Random Gestures
            mTrainer.IsPositiveTraining = false;
            const int NUM_RANDOM_GESTURES = 30;
            for (int i = 0; i < NUM_RANDOM_GESTURES; ++i)
            {
                //AddRandomData((float)MathUtils.NextRand(.8, 1.2), (int)MathUtils.NextRand(25, 35));
            }

            // Generate a NeuralNet with a single hidden node
            mGenerator.NumHidden = 1;
            BinaryGestureNeuralNetwork gestureNetworkSingle = (BinaryGestureNeuralNetwork)mGenerator.Generate();
            gestureNetworkSingle.RegisterListener(this);

            // Generate a Gesture NeuralNet with three hidden nodes
            mGenerator.NumHidden = 3;
            BinaryGestureNeuralNetwork gestureNetworkMultiple = (BinaryGestureNeuralNetwork)mGenerator.Generate();
            gestureNetworkMultiple.RegisterListener(this);

            // Train networks
            mTrainer.LearningRate = 0.17;
            mTrainer.TrainBackPropagation(gestureNetworkSingle);
            //mTrainer.TrainBackPropagation(gestureNetworkMultiple);

            // Remove Trainer as listener and add Single hidden node NeuralNet
            mScape.RemoveListener(mTrainer);
            mScape.RegisterListener(gestureNetworkSingle);

            // Pass several Punch gestures to test the single hidden node network. It should recognize each one
            for (int i = 0; i < NUM_PUNCH_GESTURES; ++i)
            {
                AddStraightData(30);

                string lastGestureDetected;
                Assert.True(GetLastGestureDetected(out lastGestureDetected));
                Assert.AreEqual("Punch", lastGestureDetected);
            }
        }

        public void OnGestureDetected(string name)
        {
            mDetectedGestureQueue.Enqueue(name);
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

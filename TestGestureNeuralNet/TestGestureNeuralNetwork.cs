using System.Collections.Generic;

namespace LoboLabs.GestureNeuralNet.Test
{
    using NUnit.Framework;
    using LoboLabs.Utilities;
    using NeuralNet;

    [TestFixture]
    public class TestGestureNeuralNetwork
    {
        private GestureManager mGestureManager;

        private const int NUM_VALUES_PER_VECTOR = 3;
        private const int NUM_OUTPUTS = 3;

        [SetUp]
        public void SetUp()
        {
            mGestureManager = new GestureManager();
        }

        public void AddData(List<Vector> gestureData)
        {
            mGestureManager.StartGesturing(GestureHand.RIGHT);

            foreach (Vector position in gestureData)
            {
                mGestureManager.UpdateGesturePosition(GestureHand.RIGHT, position);
            }

            mGestureManager.StopGesturing(GestureHand.RIGHT);
        }

        [Test]
        public void SingleGesture()
        {
        }
    }
}
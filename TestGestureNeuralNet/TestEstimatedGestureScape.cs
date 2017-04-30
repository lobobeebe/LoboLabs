namespace LoboLabs.GestureNeuralNet.Test
{
    using NeuralNet;
    using Utilities;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class TestEstimatedGestureScape
    {
        private Queue<GestureData> mDetectedGestureQueue;

        [SetUp]
        public void SetUp()
        {
            mDetectedGestureQueue = new Queue<GestureData>();
        }

        [Test]
        public void Constructor()
        {
            GestureScape scape;
            
            // Constructor should throw if the given number of estimated positions is below 2 
            Assert.DoesNotThrow(() => scape = new GestureScape(5));
            Assert.DoesNotThrow(() => scape = new GestureScape(50));
            Assert.DoesNotThrow(() => scape = new GestureScape(2));
        }

        [Test]
        public void Gesturing()
        {
            const int NUM_POSITIONS = 5;
            GestureScape scape = new GestureScape(NUM_POSITIONS);
            scape.DataReceived += ReceiveData;

            // Make sure a call to Update does not throw when called before Start
            Assert.DoesNotThrow(() => scape.UpdateGesturePosition(new Vector(0, 0, 0)));

            // Start a new gesture
            scape.StartGesturing();

            // Update with several positions
            scape.UpdateGesturePosition(new Vector(0, 0, 0));
            scape.UpdateGesturePosition(new Vector(1, 0, 0));
            scape.UpdateGesturePosition(new Vector(2, 0, 0));
            scape.UpdateGesturePosition(new Vector(2, 1, 0));
            scape.UpdateGesturePosition(new Vector(2, 2, 0));
            scape.UpdateGesturePosition(new Vector(2, 2, 1));
            scape.UpdateGesturePosition(new Vector(2, 2, 2));
            scape.UpdateGesturePosition(new Vector(2, 2, 3));
            scape.UpdateGesturePosition(new Vector(2, 2, 4));

            // Indicate gesturing has stopped
            scape.StopGesturing();

            // Make sure the scape's listener's received an estimated gesture
            GestureData data;
            Assert.True(GetLastGestureDetected(out data));
            List<double> dataList = data.AsList();
            Assert.AreEqual(NUM_POSITIONS * 3, dataList.Count);

            // The contents are tested in the EstimatedGestureData's tests

            // Make sure updating the scape with less than the expected number of positions does
            // not result in a callback.
            scape.StartGesturing();
            scape.StopGesturing();
            Assert.False(GetLastGestureDetected(out data));

            scape.UpdateGesturePosition(new Vector(1, 1, 1));
            scape.UpdateGesturePosition(new Vector(2, 2, 2));
            scape.UpdateGesturePosition(new Vector(3, 3, 3));
            scape.UpdateGesturePosition(new Vector(4, 4, 4));
            scape.UpdateGesturePosition(new Vector(5, 5, 5));
            scape.StopGesturing();
            Assert.False(GetLastGestureDetected(out data));
            
            scape.UpdateGesturePosition(new Vector(1, 1, 1));
            scape.StopGesturing();
            Assert.True(GetLastGestureDetected(out data));
            dataList = data.AsList();
            Assert.AreEqual(NUM_POSITIONS * 3, dataList.Count);
        }

        public void ReceiveData(object sender, ScapeData output)
        {
            Assert.True(output.GetType() == typeof(GestureData));

            mDetectedGestureQueue.Enqueue(output as GestureData);
        }


        public bool GetLastGestureDetected(out GestureData gestureData)
        {
            if (mDetectedGestureQueue.Count > 0)
            {
                gestureData = mDetectedGestureQueue.Dequeue();
                return true;
            }

            gestureData = null;
            return false;
        }
    }
}

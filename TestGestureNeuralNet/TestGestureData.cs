using System.Collections.Generic;
using NUnit.Framework;

namespace LoboLabs.GestureNeuralNet.Test
{
    using LoboLabs.Utilities;
    using Utilities;

    [TestFixture]
    public class TestGestureData
    {
        [Test]
        public void Constructor()
        {
            GestureData data = new GestureData(5);
            Assert.AreEqual(5, data.NumPositionsInGesture);

            data = new GestureData(10);
            Assert.AreEqual(10, data.NumPositionsInGesture);
        }

        [Test]
        public void AsList()
        {
            // Create a GestureData with 6 positions representing 5 deltas
            GestureData data = new GestureData(5);
            data.AddPosition(new Vector(0, 0, 0));
            data.AddPosition(new Vector(0, 0, 2));
            data.AddPosition(new Vector(0, 2, 2));
            data.AddPosition(new Vector(2, 2, 2));
            data.AddPosition(new Vector(2, 4, 2));
            data.AddPosition(new Vector(2, 4, 4));
            
            // Get the GestureData as a list
            List<double> list = data.AsList();
            Assert.AreEqual(15, list.Count); // 15 because 3 values per position (x, y, z)
            Assert.AreEqual(new List<double>() { 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1 }, list);

            // Create a GestureData with 9 positions representing 8 deltas that will be reduced to 4 deltas.
            data = new GestureData(4);
            data.AddPosition(new Vector(0, 0, 0));
            data.AddPosition(new Vector(1, 1, 1));
            data.AddPosition(new Vector(2, 2, 2));
            data.AddPosition(new Vector(3, 3, 3));
            data.AddPosition(new Vector(4, 4, 4));
            data.AddPosition(new Vector(5, 5, 5));
            data.AddPosition(new Vector(6, 6, 6));
            data.AddPosition(new Vector(7, 7, 7));
            data.AddPosition(new Vector(8, 8, 8));

            // Make sure the list has 12 values, (x, y, z) * 4
            list = data.AsList();
            Assert.AreEqual(12, list.Count);

            // The estimated deltas should all be 1/sqrt(3). Each leg of the vector (1, 1, 1) normalized
            double expectedValue = 1 / System.Math.Sqrt(3);
            List<double> expectedList = new List<double>()
            {
                expectedValue, expectedValue, expectedValue,
                expectedValue, expectedValue, expectedValue,
                expectedValue, expectedValue, expectedValue,
                expectedValue, expectedValue, expectedValue,
            };
            
            double delta = 0.00001;
            for(int i = 0; i < 12; ++i)
            {
                Assert.True((list[i] + delta > expectedList[i]) || (list[i] - delta < expectedList[i]));
            }
        }
    }
}

using System.Collections.Generic;
using NUnit.Framework;
using System;

namespace LoboLabs.GestureNeuralNet.Test
{
    using Utilities;

    [TestFixture]
    public class TestEstimatedGestureData
    {
        [Test]
        public void Constructor()
        {
            GestureData data;
            
            // Constructor should throw if the given number of estimated positions is below 2 
            Assert.DoesNotThrow(() => data = new GestureData(5));
            Assert.DoesNotThrow(() => data = new GestureData(50));
            Assert.DoesNotThrow(() => data = new GestureData(2));

            Assert.Throws<NotSupportedException>(() => data = new GestureData(1));
            Assert.Throws<NotSupportedException>(() => data = new GestureData(-10));
        }

        [Test]
        public void AsList()
        {
            GestureData data = new GestureData(5);
            data.AddPosition(new Vector(0, 0, 0));
            data.AddPosition(new Vector(0, 0, 2));
            data.AddPosition(new Vector(0, 2, 2));
            data.AddPosition(new Vector(2, 2, 2));
            data.AddPosition(new Vector(2, 4, 2));
            data.AddPosition(new Vector(2, 4, 4));
            
            List<double> list = data.AsList();
            Assert.AreEqual(15, list.Count);
            Assert.AreEqual(new List<double>() { 0, 0, 2, 0, 2, 0, 2, 0, 0, 0, 2, 0, 0, 0, 2 }, list);

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

            list = data.AsList();
            Assert.AreEqual(12, list.Count);
            
            List<double> expectedList = new List<double>()
            {
                2, 2, 2,
                2, 2, 2,
                2, 2, 2,
                2, 2, 2,
            };

            double delta = 0.00001;
            for(int i = 0; i < 12; ++i)
            {
                Assert.True(list[i] + delta > expectedList[i]);
                Assert.True(list[i] - delta < expectedList[i]);
            }
        }
    }
}

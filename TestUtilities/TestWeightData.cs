using NUnit.Framework;

namespace LoboLabs.Utilities.Test
{
    using Utilities;

    [TestFixture]
    public class TestWeightData
    {
        private WeightData data = new WeightData();

        [Test]
        public void Weight()
        {
            data.Weight = 10.0;
            Assert.AreEqual(10.0, data.Weight);

            data.Weight = 0.3;
            Assert.AreEqual(0.3, data.Weight);
        }
        
        [Test]
        public void TotalWeightDelta()
        {
            data.TotalWeightDelta = 10.0;
            Assert.AreEqual(10.0, data.TotalWeightDelta);

            data.TotalWeightDelta = 0.3;
            Assert.AreEqual(0.3, data.TotalWeightDelta);
        }
    }
}

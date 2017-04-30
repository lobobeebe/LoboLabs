using NUnit.Framework;

namespace LoboLabs.NeuralNet.Test
{
    [TestFixture]
    public class TestScape
    {
        private uint mNumDataReceived;

        [Test]
        public void Constructor()
        {
            Mock.Scape scape = new Mock.Scape();
            scape.DataReceived += OnDataReceived;
            
            for (int i = 1; i <= 10; ++i)
            {
                Mock.ScapeData data = new Mock.ScapeData();
                scape.NotifyDataReceived(data);
                Assert.AreEqual(i, mNumDataReceived);
            }
        }

        public void OnDataReceived(object Sender, ScapeData scapeData)
        {
            mNumDataReceived++;
        }
    }
}

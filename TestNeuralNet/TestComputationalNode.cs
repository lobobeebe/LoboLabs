using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using LoboLabs.Utilities;

namespace LoboLabs.NeuralNet.Test
{
    [TestFixture]
    public class TestCompuatationalNode
    {
        List<Node> mInputNodes;

        Mock.ComputationalNode mNode;

        [SetUp]
        public void SetUp()
        {
            mInputNodes = new List<Node>();
            mNode = new Mock.ComputationalNode(new Functions.HyperbolicTangent());
        }

        [Test]
        public void ComputeAndCalculateSingleInput()
        {
            //-----------------------------------------------------------------------------**
            // Check for each of the following
            // 
            // Compute output = tanh(weight * input)
            //
            // Weight' = Weight * a * s * f'(Output) * input
            //         = Weight * a * s * (1 + output) * (1 - output) * input
            //
            // Input's Error Signal = s * Weight * (1 + output) * (1 - output)
            //
            //      where:
            //             a is learning rate
            //             s is error signal sum
            //-----------------------------------------------------------------------------**
            Node inputNode = new Node(true);

            //-----------------------------------------------------------------------------**
            // Weight .5, Input .5
            // Target 1.5, Learning Rate .1
            //-----------------------------------------------------------------------------**

            // output = tanh(.5 * .5) = 0.244918
            mNode.RegisterInput(inputNode, .5);
            inputNode.LastOutput = .5;
            mNode.Compute();
            double previousOutput = mNode.LastOutput;
            Assert.AreEqual(0.244918, mNode.LastOutput, .000001);

            // Verify that for 100 cycles, the output of the node is getting closer and closer to the desired target
            double target = 1.5;
            for (int i = 0; i < 100; ++i)
            {
                mNode.Compute();
                Assert.IsTrue(Math.Abs(mNode.LastOutput - target) <= Math.Abs(previousOutput - target));
                previousOutput = mNode.LastOutput;

                mNode.AddErrorSignal(mNode.LastOutput - target);
                mNode.CalculateErrorSignals();
                mNode.UpdateWeightDeltas(.1);
            }
        }        

        [Test]
        public void SaveAndLoad()
        {
            int numInputs = 5;
            List<uint> uuidList = new List<uint>();
            Dictionary<Node, double> uuidWeightMap = new Dictionary<Node, double>();

            // Create Inputs
            for (int i = 0; i < numInputs; ++i)
            {
                Node node = new Node(true);
                uuidWeightMap.Add(node, i);
                uuidList.Add(node.UUID);
                
                mNode.RegisterInput(node, i);
            }

            // Set Bias
            mNode.Bias = 10;
            
            // Using MemoryStream 
            using (MemoryStream stream = new MemoryStream())
            {
                // Write the node to the Memory Stream
                BinaryWriter writer = new BinaryWriter(stream);
                mNode.Save(writer);

                // Read the node from the Memory Stream
                stream.Position = 0;
                BinaryReader reader = new BinaryReader(stream);
                ComputationalNode loadedNode = new Mock.ComputationalNode(reader);

                Assert.IsTrue(mNode.Equals(loadedNode));
            }

            // Verify the Node loaded back in correctly.
            Assert.AreEqual(5, mNode.InputWeights.Count);
            foreach (KeyValuePair<Node, WeightData> pair in mNode.InputWeights)
            {
                Assert.True(uuidList.Contains(pair.Key.UUID));
                Assert.AreEqual(pair.Value.Weight, uuidWeightMap[pair.Key], .00001);
            }

            Assert.AreEqual(10, mNode.Bias);
        }
    }
}

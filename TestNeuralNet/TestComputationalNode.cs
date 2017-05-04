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

        NeuralNetwork mParent;
        ComputationalNode mNode;

        [SetUp]
        public void SetUp()
        {
            mInputNodes = new List<Node>();
            mParent = new NeuralNetwork();
            mNode = new ComputationalNode(mParent, new Functions.HyperbolicTangent());
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
            const int NUM_INPUTS = 5;

            List<Node> sensors = new List<Node>();

            // Create Inputs
            for (int i = 0; i < NUM_INPUTS; ++i)
            {
                Node node = new Node(true);
                sensors.Add(node);
                
                mNode.RegisterInput(node, i);
            }

            mParent.Sensors = sensors;

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
                ComputationalNode loadedNode = new ComputationalNode(mParent, reader);

                Assert.IsTrue(mNode.Equals(loadedNode));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using NUnit.Framework;

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
            Node inputNode = new Node();

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

            // weight delta = .1 * (1.5 - mNode.LastOutput) * (1 + 0.25) * (1 - 0.25) * .5 = .070312
            mNode.AddErrorSignal(1.5);
            mNode.CalculateErrorSignals();
            mNode.UpdateWeightDeltas(.1);
            Assert.AreEqual(0.570312, mNode.InputWeights[inputNode].Weight, .000001);

            // Input's Error Signal = (1.5 - mNode.LastOutput) * .5 * (1 + 0.25) * (1 - 0.25) = .703125
            Assert.AreEqual(0.703125, inputNode.GetAndResetErrorSignalSum(), .000001);

            // Verify that for 100 cycles, the output of the node is getting closer and closer to the desired target
            double target = .9;
            for (int i = 0; i < 100; ++i)
            {
                mNode.Compute();
                Assert.IsTrue(Math.Abs(target - mNode.LastOutput) < Math.Abs(target - previousOutput));
                previousOutput = mNode.LastOutput;

                mNode.AddErrorSignal(target - mNode.LastOutput);
                mNode.CalculateErrorSignals();
                mNode.UpdateWeightDeltas(1);
            }

            //-----------------------------------------------------------------------------**
            // Weight .1, Input .9
            // ErrorSignal 40, Learning Rate .2
            //-----------------------------------------------------------------------------**

            // output = tanh(.1 * .9) = 0.089757
            mNode = new Mock.ComputationalNode(new Functions.HyperbolicTangent());
            inputNode.GetAndResetErrorSignalSum();
            mNode.RegisterInput(inputNode, .1);
            inputNode.LastOutput = .9;
            mNode.Compute();
            Assert.AreEqual(0.089757, mNode.LastOutput, .000001);

            // weight delta = .2 * 40 * (1 + 0.09) * (1 - 0.09) * .9 = 7.241680
            mNode.AddErrorSignal(40);
            mNode.CalculateErrorSignals();
            mNode.UpdateWeightDeltas(.2);
            Assert.AreEqual(7.241680, mNode.InputWeights[inputNode].Weight, .000001);

            // Input's Error Signal = 40 * .1 * (1 + 0.09) * (1 - 0.09) = 3.9676
            Assert.AreEqual(3.9676, inputNode.GetAndResetErrorSignalSum(), .000001);

            //-----------------------------------------------------------------------------**
            // Weight .3, Input 2
            // ErrorSignal .01, Learning Rate 1
            //-----------------------------------------------------------------------------**

            // output = tanh(.3 * 2) = 0.537049
            mNode = new Mock.ComputationalNode(new Functions.HyperbolicTangent());
            mNode.RegisterInput(inputNode, .3);
            inputNode.LastOutput = 2;
            mNode.Compute();
            Assert.AreEqual(0.537049, mNode.LastOutput, .000001);

            // weight delta = 1 * .01 * (1 + 0.6) * (1 - 0.6) * 2 = 0.31280
            mNode.AddErrorSignal(.01);
            mNode.CalculateErrorSignals();
            mNode.UpdateWeightDeltas(1);
            Assert.AreEqual(0.31280, mNode.InputWeights[inputNode].Weight, .000001);

            // Input's Error Signal = .01 * .3 * (1 + 0.6) * (1 - 0.6) = 0.00192
            Assert.AreEqual(0.00192, inputNode.GetAndResetErrorSignalSum(), .000001);
        }

        [Test]
        public void ComputeAndCalculateMultipleInputs()
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
            Node inputNode0 = new Node();
            Node inputNode1 = new Node();
            Node inputNode2 = new Node();

            //-----------------------------------------------------------------------------**
            // Weight .5, Input .5
            // Weight .1, Input .9
            // Weight .3, Input .2
            // ErrorSignal .4, Learning Rate .6
            //-----------------------------------------------------------------------------**

            // output = tanh(.5 * .5 + .1 * .9 + .3 * .2) = 0.379948
            mNode.RegisterInput(inputNode0, .5);
            mNode.RegisterInput(inputNode1, .1);
            mNode.RegisterInput(inputNode2, .3);
            inputNode0.LastOutput = .5;
            inputNode1.LastOutput = .9;
            inputNode2.LastOutput = .2;
            mNode.Compute();
            double firstOutput = mNode.LastOutput;
            Assert.AreEqual(0.379948, mNode.LastOutput, .000001);

            // weight0 delta = .6 * .4 * (1 + 0.4) * (1 - 0.4) * .5 = 0.1008
            mNode.AddErrorSignal(.4);
            mNode.CalculateErrorSignals();
            mNode.UpdateWeightDeltas(.6);
            Assert.AreEqual(0.6008, mNode.InputWeights[inputNode0].Weight, .000001);

            // Input0's Error Signal = .4 * .5 * (1 + 0.4) * (1 - 0.4) = .168
            Assert.AreEqual(.168, inputNode0.GetAndResetErrorSignalSum(), .000001);

            // weight1 delta = .6 * .4 * (1 + 0.4) * (1 - 0.4) * .9 = 0.18144
            Assert.AreEqual(0.28144, mNode.InputWeights[inputNode1].Weight, .000001);

            // Input1's Error Signal = .4 * .1 * (1 + 0.4) * (1 - 0.4) = 0.0336
            Assert.AreEqual(0.0336, inputNode1.GetAndResetErrorSignalSum(), .000001);

            // weight2 delta = .6 * .4 * (1 + 0.4) * (1 - 0.4) * .2 = 0.04032
            Assert.AreEqual(0.34032, mNode.InputWeights[inputNode2].Weight, .000001);

            // Input2's Error Signal = .4 * .3 * (1 + 0.4) * (1 - 0.4) = 0.1008
            Assert.AreEqual(0.1008, inputNode2.GetAndResetErrorSignalSum(), .000001);

        }
    }
}

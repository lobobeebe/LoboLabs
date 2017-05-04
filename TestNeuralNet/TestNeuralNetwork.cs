using System.Collections.Generic;

namespace LoboLabs.NeuralNet.TestNeuralNetwork
{
    using NUnit.Framework;
    using System.IO;

    [TestFixture]
    public class TestNeuralNetwork
    {
        private NeuralNetwork mNeuralNet;

        [Test]
        public void SaveAndLoad()
        {
            const int NUM_INPUTS = 5;
            const int NUM_OUTPUTS = 5;

            NeuralNetworkCreator networkCreator = new NeuralNetworkCreator();

            mNeuralNet = networkCreator.Create(NUM_INPUTS, NUM_OUTPUTS);
            
            // Using MemoryStream 
            using (MemoryStream stream = new MemoryStream())
            {
                // Write the node to the Memory Stream
                BinaryWriter writer = new BinaryWriter(stream);
                mNeuralNet.Save(writer);
                
                // Read the node from the Memory Stream
                stream.Position = 0;
                BinaryReader reader = new BinaryReader(stream);
                NeuralNetwork loadedNet = new NeuralNetwork(reader);

                Assert.True(mNeuralNet.Equals(loadedNet));
            }
        }
    }
    
    /*
    [TestFixture]
    public class TestNeuralNetwork
    {
        private Mock.Scape mScape;
        private NeuralNetwork mNeuralNet;

        [SetUp]
        public void SetUp()
        {
            mScape = new Mock.Scape();
            mNeuralNet = new NeuralNetwork();
        }
        
        [Test]
        public void ComputeSingleNodeSingleSensor()
        {
            // Add single sensor
            mNeuralNet.Sensors.Add(new Node());

            // Add single Node
            mNeuralNet.Neurons.Add(new List<ComputationalNode>());
            mNeuralNet.Neurons[0].Add(new ComputationalNode(new Functions.NoOpActivationFunction()));

            // Add the sensor as an input to the single node
            mNeuralNet.Neurons[0][0].RegisterInput(mNeuralNet.Sensors[0], 1);

            // Compute with null inputs
            List<double> inputs = null;
            Assert.DoesNotThrow(() => mNeuralNet.Compute(inputs), "Compute threw on NULL inputs");

            // Compute with empty inputs
            inputs = new List<double>();
            Assert.AreEqual(0, mNeuralNet.Compute(inputs));

            // Compute with too many inputs
            inputs.Add(.5);
            inputs.Add(.1);
            Assert.AreEqual(0, mNeuralNet.Compute(inputs));

            // Compute with correctly formatted inputs
            // With no op activation function, output should be input
            inputs.Clear();
            inputs.Add(.5);
            Assert.AreEqual(.5, mNeuralNet.Compute(inputs));
            inputs[0] = .1;
            Assert.AreEqual(.1, mNeuralNet.Compute(inputs));
            inputs[0] = .9;
            Assert.AreEqual(.9, mNeuralNet.Compute(inputs));
        }

        [Test]
        public void ComputeSingleNodeMultipleSensors()
        {
            // Add multiple sensors
            mNeuralNet.Sensors.Add(new Node());
            mNeuralNet.Sensors.Add(new Node());
            mNeuralNet.Sensors.Add(new Node());

            // Add single Node
            mNeuralNet.Neurons.Add(new List<ComputationalNode>());
            mNeuralNet.Neurons[0].Add(new ComputationalNode(new Functions.HyperbolicTangent()));

            // Add the sensors as an input to the single node
            mNeuralNet.Neurons[0][0].RegisterInput(mNeuralNet.Sensors[0], .1);
            mNeuralNet.Neurons[0][0].RegisterInput(mNeuralNet.Sensors[1], .3);
            mNeuralNet.Neurons[0][0].RegisterInput(mNeuralNet.Sensors[2], .6);

            // Compute - Results verified by calculator
            // result = tanh[(.1 * i0) + (.3 * i1) + (.6 * i2)]
            List<double> inputs = new List<double>();

            // result = tanh[(.1 * 1) + (.3 * 1) + (.6 * 1)] = tanh(1) = 0.761594
            inputs.Add(1);
            inputs.Add(1);
            inputs.Add(1);
            List<double> expectedOutputs = new List<double>(new double[] { 0, 0, 0.761594 });
            //Assert.AreEqual 
            //Assert.AreEqual(, mNeuralNet.Compute(inputs), .000001);

            // result = tanh[(.1 * .6) + (.3 * .3) + (.6 * .1)] = tanh(.21) = 0.206966
            inputs.Clear();
            inputs.Add(.6);
            inputs.Add(.3);
            inputs.Add(.1);
            Assert.AreEqual(0.206966, mNeuralNet.Compute(inputs), .000001);
        }

        [Test]
        public void ComputeMultipleNodesMultipleSensors()
        {
            // Add multiple sensors
            mNeuralNet.Sensors.Add(new Node());
            mNeuralNet.Sensors.Add(new Node());
            mNeuralNet.Sensors.Add(new Node());

            // Add two Neuron layers
            mNeuralNet.Neurons.Add(new List<ComputationalNode>());
            mNeuralNet.Neurons.Add(new List<ComputationalNode>());

            mNeuralNet.Neurons[0].Add(new ComputationalNode(new Functions.HyperbolicTangent()));
            mNeuralNet.Neurons[0].Add(new ComputationalNode(new Functions.HyperbolicTangent()));

            mNeuralNet.Neurons[1].Add(new ComputationalNode(new Functions.HyperbolicTangent()));

            // Add the sensors as an input to each first layer node
            foreach (Node sensor in mNeuralNet.Sensors)
            {
                mNeuralNet.Neurons[0][0].RegisterInput(sensor, .1);
                mNeuralNet.Neurons[0][1].RegisterInput(sensor, .2);
            }

            // Add each first layer node as input to the second layer node
            mNeuralNet.Neurons[1][0].RegisterInput(mNeuralNet.Neurons[0][0], .3);
            mNeuralNet.Neurons[1][0].RegisterInput(mNeuralNet.Neurons[0][1], .4);

            // Compute - Results verified by calculator
            // First Layer
            // result0 = tanh[(.1 * i0) + (.1 * i1) + (.1 * i2)]
            // result1 = tanh[(.2 * i0) + (.2 * i1) + (.2 * i2)]
            // Second Layer
            // result2 = tanh[(.3 * result0) + (.4 * result1)]
            List<double> inputs = new List<double>();

            // First Layer
            // result0 = tanh[(.1 * 1) + (.1 * 1) + (.1 * 1)] = 0.291312
            // result1 = tanh[(.2 * 1) + (.2 * 1) + (.2 * 1)] = 0.537049
            // Second Layer
            // result2 = tanh[(.3 * 0.291312) + (.4 * 0.537049)] = 0.452141
            inputs.Add(1);
            inputs.Add(1);
            inputs.Add(1);
            Assert.AreEqual(0.293337, mNeuralNet.Compute(inputs), .000001);

            // First Layer
            // result0 = tanh[(.1 * .6) + (.1 * .3) + (.1 * .1)] = 0.099667
            // result1 = tanh[(.2 * .6) + (.2 * .3) + (.2 * .1)] = 0.197375
            // Second Layer
            // result2 = tanh[(.3 * 0.761594) + (.4 * 0.964027)] = 0.108422
            inputs.Clear();
            inputs.Add(.6);
            inputs.Add(.3);
            inputs.Add(.1);
            Assert.AreEqual(0.108422, mNeuralNet.Compute(inputs), .000001);
        }
    }
    */
}

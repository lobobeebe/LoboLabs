namespace LoboLabs.GestureNeuralNet
{
    using NeuralNet;
    using NeuralNet.Functions;
    using System.Collections.Generic;
    using Utilities;

    /// <summary>
    /// Class to abstract detection algorithms from the user.
    /// TODO: Give more description.
    /// </summary>
    public class GestureDetectionManager
    {
        private static ClassLogger Logger = new ClassLogger(typeof(GestureDetectionManager));

        private const float DEFAULT_CONFIDENCE_INTERVAL = .75f;
        private const string DEFAULT_GESTURE_NAME = "Default";
        private const int DEFAULT_NUM_POSITIONS_IN_GESTURES = 11;

        /// <summary>
        /// Constructor
        /// </summary>
        public GestureDetectionManager()
        {
            IsTraining = true;
            
            // Estimate Training Gesture Map
            RightHandTrainingGestureDataMap = new Dictionary<string, List<GestureData>>();

            // Estimate Current Gesture Data for each item
            CurrentRightHandGestureData = new GestureData();

            // Initialize Current Training Names for each item
            NameOfRightHandGestureBeingTrained = DEFAULT_GESTURE_NAME;

            // Initialize the serializer that will turn Gesture Data into raw values for the Network
            GestureSerializer = new EstimatedGestureDataSerializer(DEFAULT_NUM_POSITIONS_IN_GESTURES);

            // Initialize the default Confidence Threshold for gesture detections
            MinimumConfidenceThreshold = DEFAULT_CONFIDENCE_INTERVAL;
        }

        /// <summary>
        /// TODO
        /// </summary>
        private GestureData CurrentRightHandGestureData
        {
            get;
            set;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public delegate void GestureDetectionEventHandler(object sender, GestureDetectionEvent args);

        private GestureDataSerializer GestureSerializer
        {
            get;
            set;
        }

        /// <summary>
        /// TODO
        /// </summary>
        private bool IsTraining
        {
            get;
            set;
        }

        public void FinalizeTraining()
        {
            if (RightHandTrainingGestureDataMap.Count > 0)
            {
                List<string> trainedGestureNames = new List<string>(RightHandTrainingGestureDataMap.Keys);

                // Number of inputs will be the number of changes in a gesture (n - 1, where n is the number of entries in the gesture)
                // multiplied by the size of each entry
                // TODO: Allow the user to change the number of desired positions to store
                int numInputs = DEFAULT_NUM_POSITIONS_IN_GESTURES * GestureData.SIZE_PER_ENTRY;

                // Generate a NeuralNetwork
                NeuralNetworkGenerator generator = new NeuralNetworkGenerator();
                RightHandNeuralNet = generator.Generate(numInputs, trainedGestureNames.Count);
                RightHandNeuralNet.ResultReceived += OnRightHandResultReceived;

                // Train the network
                NeuralNetworkTrainer trainer = new NeuralNetworkTrainer(new SumSquaredError(), numInputs, trainedGestureNames);

                foreach (KeyValuePair<string, List<GestureData>> keyValuePair in RightHandTrainingGestureDataMap)
                {
                    trainer.CurrentOutputName = keyValuePair.Key;
                    
                    foreach(GestureData data in keyValuePair.Value)
                    {
                        List<double> rawGesture = GestureSerializer.Serialize(data);

                        trainer.ProcessData(rawGesture);
                    }
                }

                trainer.TrainBackPropagation(RightHandNeuralNet);

                // Indicate that training is now complete
                IsTraining = false;
            }
            else
            {
                Logger.Error("Cannot finalize training for right hand with no training data.");
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private float MinimumConfidenceThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// TODO
        /// </summary>
        private string NameOfRightHandGestureBeingTrained
        {
            get;
            set;
        }
        
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void OnRightHandResultReceived(object sender, List<double> input, List<double> output)
        {
            int maxIndex = VectorUtils.MaxIndex(output);

            if (maxIndex >= 0 && output[maxIndex] > MinimumConfidenceThreshold)
            {
                // Get the list of gesture names from the training data
                List<string> gestureNames = new List<string>(RightHandTrainingGestureDataMap.Keys);

                // Indicate that a gesture was detected.
                RightHandGestureDetected(this, new GestureDetectionEvent(gestureNames[maxIndex],
                    GestureSerializer.Deserialize(input), output[maxIndex]));
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        public event GestureDetectionEventHandler RightHandGestureDetected;

        /// <summary>
        /// TODO 
        /// </summary>
        private NeuralNetwork RightHandNeuralNet
        {
            get;
            set;
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void StartRightHandGesturing()
        {
            // Reset the current gesture data for the given item
            CurrentRightHandGestureData = new GestureData();
        }
        
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="gestureName"></param>
        public void StartTrainingRightHandGestureName(string gestureName)
        {
            NameOfRightHandGestureBeingTrained = gestureName;

            // This will reset the entire system back to training mode.
            IsTraining = true;
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void StopRightHandGesturing()
        {
            // If training, add the current gesture to the trained gestures.
            // If not currently training, feed the current gesture to the network.
            if (IsTraining)
            {   
                // If no entry for the current gesture name exists, create a new list
                if (!RightHandTrainingGestureDataMap.ContainsKey(NameOfRightHandGestureBeingTrained))
                {
                    RightHandTrainingGestureDataMap.Add(NameOfRightHandGestureBeingTrained,
                        new List<GestureData>());
                }

                // Add the Current Gesture data to the training map
                RightHandTrainingGestureDataMap[NameOfRightHandGestureBeingTrained].Add(CurrentRightHandGestureData);
            }
            else
            {
                // Feed the Gesture to the Neural Network
                if(RightHandNeuralNet != null)
                {
                    // Serialize the Gesture Data into raw values
                    List<double> serializedPositions = GestureSerializer.Serialize(CurrentRightHandGestureData);

                    // Compute on the gesture
                    RightHandNeuralNet.ProcessData(this, serializedPositions);
                }
                else
                {
                    Logger.Error("Can not detect gestures with a null Neural Net.");
                }
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private Dictionary<string, List<GestureData>> RightHandTrainingGestureDataMap
        {
            get;
            set;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="gesturingItem"></param>
        /// <param name="location"></param>
        public void UpdateRightHandLocation(Vector location)
        {
            CurrentRightHandGestureData.AddPosition(location);
        }
    }
}

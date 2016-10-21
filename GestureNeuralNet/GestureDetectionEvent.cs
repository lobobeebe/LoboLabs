namespace LoboLabs.GestureNeuralNet
{ 
    /// <summary>
    /// Represents a gesture
    /// </summary>
    public class GestureDetectionEvent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gestureName"></param>
        /// <param name="gestureData"></param>
        /// <param name="confidence"></param>
        public GestureDetectionEvent(string gestureName, GestureData gestureData, double confidence)
        {
            Name = gestureName;
            Gesture = gestureData;
            Confidence = confidence;
        }

        /// <summary>
        /// TODO
        /// </summary>
        public double Confidence
        {
            get;
            private set;
        }

        /// <summary>
        /// TODO
        /// </summary>
        public GestureData Gesture
        {
            get;
            private set;
        }

        /// <summary>
        /// TODO
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
    }

}

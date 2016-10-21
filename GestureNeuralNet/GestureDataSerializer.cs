namespace LoboLabs.GestureNeuralNet
{
    using System.Collections.Generic;
    using Utilities;

    /// <summary>
    /// Class that serializes GestureData into a list of raw values.
    /// Subclasses will inherit from this to alter thae way in which gesture data is serialized.
    /// </summary>
    public class GestureDataSerializer
    {
        private static ClassLogger Logger = new ClassLogger(typeof(GestureDataSerializer));

        /// <summary>
        /// Constructor
        /// </summary>
        public GestureDataSerializer()
        {
        }

        public virtual GestureData Deserialize(List<double> serializedGestureData)
        {
            GestureData data = new GestureData();
            
            for (int i = 0; i < serializedGestureData.Count; i += 3)
            {
                if (serializedGestureData.Count - i >= 3)
                {
                    data.AddPosition(new Vector((float)serializedGestureData[i], (float)serializedGestureData[i + 1],
                        (float)serializedGestureData[i + 2]));
                }
                else
                {
                    Logger.Warn("Left over values at the end of deserialization. Likely a malformed gesture.");
                }
            }

            return data;
        }
        
        /// <summary>
        /// Serializes GestureData into a list of values 
        /// </summary>
        public virtual List<double> Serialize(GestureData gestureData)
        {
            List<double> convertedData = new List<double>();

            // Add the X, Y, and Z of each position in the list
            foreach (Vector position in gestureData.Positions)
            {
                convertedData.Add(position.X);
                convertedData.Add(position.Y);
                convertedData.Add(position.Z);
            }

            return convertedData;
        }
    }
}

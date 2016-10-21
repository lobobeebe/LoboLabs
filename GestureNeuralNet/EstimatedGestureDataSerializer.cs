using System;
using System.Collections.Generic;
using LoboLabs.Utilities;

namespace LoboLabs.GestureNeuralNet
{ 
    /// <summary>
    /// Class that serializes GestureData into a list of raw values.
    /// Subclasses will inherit from this to alter thae way in which gesture data is serialized.
    /// </summary>
    public class EstimatedGestureDataSerializer : GestureDataSerializer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numPositions"></param>
        public EstimatedGestureDataSerializer(int numPositions)
        {
            NumPositions = numPositions;
        }

        private int NumPositions
        {
            get;
            set;
        }

        /// <summary>
        /// Serializes GestureData into a list of values 
        /// </summary>
        public override List<double> Serialize(GestureData gestureData)
        {
            List<double> convertedData = new List<double>();

            // TODO: Add a way to push points on if the count is less than the NumPositions
            if (gestureData.Positions.Count > NumPositions)
            {
                List<Vector> localPositions = new List<Vector>(gestureData.Positions);

                // TODO: Can this be more efficient?
                // Estimate the gesture down to the set number of points + 1 in order to 
                // result in 'points' number of direction vectors
                while (localPositions.Count > NumPositions + 1)
                {
                    // Pushback estimation
                    // * For each point index, i, push point i by (i / (n - 2)) times the difference between the points
                    // * Remove the last point
                    for (int datum = 1; datum < localPositions.Count - 1; ++datum)
                    {
                        Vector difference = (localPositions[datum + 1] - localPositions[datum]);
                        localPositions[datum] += difference * ((float)datum / (localPositions.Count - 2));
                    }

                    localPositions.RemoveAt(localPositions.Count - 1);
                }

                // Convert to directional vectors
                for (int datum = 0; datum < localPositions.Count - 1; ++datum)
                {
                    Vector difference = (localPositions[datum + 1] - localPositions[datum]);
                    convertedData.AddRange(difference.ToList());
                }
            }

            return convertedData;
        }
    }
}

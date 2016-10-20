using System;
using System.Collections.Generic;
using LoboLabs.NeuralNet;
using LoboLabs.Utilities;

namespace LoboLabs.GestureNeuralNet
{ 
    /// <summary>
    /// Represents an estimated gesture. Will hold all positions given and estimate the gesture as a list.
    /// </summary>
    public class EstimatedGestureData : ScapeData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numPositions">The number of positions down to which all gestures will be estimated</param>
        public EstimatedGestureData(int numPositions)
        {
            // Throw for too few estimated positions
            if (numPositions < 2)
            {
                throw new NotSupportedException("The number of estimated positions must not be less than 2.");
            }

            NumPositions = numPositions;
            Positions = new List<Vector>();
        }

        /// <summary>
        /// Adds a position to the gesture information.
        /// </summary>
        /// <param name="position">The position to add</param>
        public void AddPosition(Vector position)
        {
            Positions.Add(position);
        }

        /// <summary>
        /// The number of positions down to which all gestures will be estimated.
        /// </summary>
        private int NumPositions
        {
            get;
            set;
        }

        /// <summary>
        /// The current list of positions comprising the gesture.
        /// </summary>
        private List<Vector> Positions
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the Gesture Data as a list of doubles representing an estimated version of the gesture.
        /// This function estimates the gestures using Pushback Estimation
        /// </summary>
        /// <returns></returns>
        public List<double> AsList()
        {
            List<double> convertedData = new List<double>();

            if (Positions.Count > NumPositions)
            {
                List<Vector> localPositions = new List<Vector>(Positions);

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

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
        /// This function estimates the gestures using Midpoint Estimation. In Iterative Midpoint Estimation,
        /// each iteration reduces the number of positions by 1. A single iteration is as follows:
        /// * Create a new vector of positions, localPositions
        /// * Add the first position of the original gesture to localPositions
        /// * For each point, i, where i = 1 -> numLocalPositions - 2
        /// ** Add the midpoint between i and i + 1 to localPositions
        /// This process is repeated until the desired number of positions in the estimated gesture is reached.
        /// Finally, the estimated gesture is converted into directional vectors.
        /// A directional vector is simply the normalized difference between two points.
        /// </summary>
        /// <returns></returns>
        public List<double> AsList()
        {
            List<Vector> localPositions = new List<Vector>(Positions);
            List<double> convertedData = new List<double>();

            // TODO: Can this be more efficient?
            // Estimate the gesture down to the set number of points + 1 in order to 
            // result in 'points' number of direction vectors
            while (localPositions.Count > NumPositions + 1)
            {
                List<Vector> estimatedGesture = new List<Vector>(localPositions.Count - 1);

                // Add first position
                estimatedGesture.Add(localPositions[0]);

                // Midpoint estimation
                for (int datum = 1; datum < localPositions.Count - 2; ++datum)
                {
                    Vector midpoint = ((localPositions[datum + 1] - localPositions[datum]) / 2)
                        + localPositions[datum];
                    estimatedGesture.Add(midpoint);
                }

                // Add last position
                estimatedGesture.Add(localPositions[localPositions.Count - 1]);

                localPositions = estimatedGesture;
            }

            // Convert to directional vectors
            for (int datum = 0; datum < localPositions.Count - 1; ++datum)
            {
                Vector difference = (localPositions[datum + 1] - localPositions[datum]).Normalized();
                convertedData.AddRange(difference.ToList());
            }

            return convertedData;
        }
    }

}

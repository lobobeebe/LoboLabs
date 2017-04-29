using System;
using System.Collections.Generic;
using LoboLabs.NeuralNet;
using LoboLabs.Utilities;
using System.IO;

namespace LoboLabs.GestureNeuralNet
{ 
    /// <summary>
    /// Represents an estimated gesture. Will hold all positions given and estimate the gesture as a list.
    /// </summary>
    public class GestureData : ScapeData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numPositions">The number of positions down to which all gestures will be estimated</param>
        public GestureData(int numPositions)
        {
            // Throw for too few estimated positions
            if (numPositions < 2)
            {
                throw new NotSupportedException("The number of estimated positions must not be less than 2.");
            }

            NumPositionsInGesture = numPositions;
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
        public int NumPositionsInGesture
        {
            get;
            private set;
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

            if (Positions.Count > NumPositionsInGesture)
            {
                List<Vector> localPositions = new List<Vector>(Positions);

                // TODO: Can this be more efficient?
                // Estimate the gesture down to the set number of points + 1 in order to 
                // result in 'points' number of direction vectors
                while (localPositions.Count > NumPositionsInGesture + 1)
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
                    Vector difference = (localPositions[datum + 1] - localPositions[datum]).Normalized();
                    convertedData.AddRange(difference.ToList());
                }
            }

            return convertedData;
        }
        
        /// <summary>
        /// Saves the Gesture Data to a file.
        /// </summary>
        /// <returns></returns>
        public void WriteToStream(BinaryWriter writer)
        {
            // Write the length of the Positions vector
            writer.Write(Positions.Count);

            // Write each x, y, z of the positions
            foreach (Vector vector in Positions)
            {
                writer.Write(vector.X);
                writer.Write(vector.Y);
                writer.Write(vector.Z);
            }
        }

        /// <summary>
        /// Loads the Gesture Data from a file.
        /// </summary>
        /// <returns></returns>
        public void LoadFromStream(BinaryReader reader)
        {
            // Clear the current gesture
            Positions.Clear();

            // Read the length of the Positions vector
            int numPositions = reader.ReadInt32();

            // Read each x, y, z of the positions
            for (int i = 0; i < numPositions; ++i)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();

                AddPosition(new Vector(x, y, z));
            }
        }
    }
}

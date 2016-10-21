using System.Collections.Generic;
using LoboLabs.Utilities;

namespace LoboLabs.GestureNeuralNet
{ 
    /// <summary>
    /// Represents a gesture
    /// </summary>
    public class GestureData
    {
        // There are three values in each entry (Vector).
        public const int SIZE_PER_ENTRY = 3; 

        private List<Vector> mPositions;

        /// <summary>
        /// Constructor
        /// </summary>
        public GestureData()
        {
            mPositions = new List<Vector>();
        }

        /// <summary>
        /// Adds a position to the gesture information.
        /// </summary>
        /// <param name="position">The position to add</param>
        public void AddPosition(Vector position)
        {
            mPositions.Add(position);
        }

        /// <summary>
        /// The current list of positions comprising the gesture.
        /// </summary>
        public IList<Vector> Positions
        {
            get
            {
                return mPositions.AsReadOnly();
            }
        }
    }

}

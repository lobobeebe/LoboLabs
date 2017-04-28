using System;

namespace LoboLabs.GestureNeuralNet
{
    using NeuralNet;
    using Utilities;

    /// <summary>
    /// Data Scape for estimated gestures. Neural Networks require the same number of inputs every iteration.
    /// Since it can't be guaranteed that a gesture will be exactly the same number of positions each iteration, 
    /// this class is used to estimate every gesture down to a set number of positions by Midpoint Estimation.
    /// </summary>
    public class GestureScape : Scape
    {
        private static ClassLogger Logger = new ClassLogger(typeof(GestureScape));
        
        /// <summary>
        /// Constructor. Takes a number of positions to which each input gesture will be estimated.
        /// </summary>
        /// <param name="numPositions">The number of positions down to which gestures will be estimated.</param>
        /// <throws type="NotSupportedException">If the number of given positions is less than 2.</throws>
        public GestureScape(int numPositions)
        {
            // Throw for too few estimated positions
            if (numPositions < 2)
            {
                throw new NotSupportedException("The number of estimated positions must not be less than 2.");
            }

            NumPositions = numPositions;

            // Initializes the current gesture and the current number of positions
            StartGesturing();
        }

        /// <summary>
        /// The current gesture information.
        /// </summary>
        private GestureData CurrentGesture
        {
            get;
            set;
        }

        /// <summary>
        /// The number of positions updated since the last 'StartGesturing' was called.
        /// If 'StartGesturing' was never called, the number of positions updated since initialization.
        /// </summary>
        private int CurrentNumPositions
        {
            get;
            set;
        }

        /// <summary>
        /// The number of positions down to which gestures will be estimated.
        /// </summary>
        private int NumPositions
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates to the scape that gesture information will now be input.
        /// </summary>
        public void StartGesturing()
        {
            // Clear the Current Gesture
            CurrentGesture = new GestureData(NumPositions);
            CurrentNumPositions = 0;
        }

        /// <summary>
        /// Indicates to the scape that gesture information has concluded.
        /// Will notify any listeners of gesture data if the number of positions required is exceeded
        /// </summary>
        public void StopGesturing()
        {
            if (CurrentNumPositions > NumPositions)
            {
                NotifyDataReceived(CurrentGesture);
            }
        }

        /// <summary>
        /// Adds a position point to the current gesture.
        /// Undefined behavior if called without a preceeding "StartGesturing"
        /// </summary>
        /// <param name="position">The new position to add to the current gesture.</param>
        public void UpdateGesturePosition(Vector position)
        {
            CurrentGesture.AddPosition(position);
            CurrentNumPositions++;
        }
    }

}
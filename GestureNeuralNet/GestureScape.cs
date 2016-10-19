using LoboLabs.NeuralNet;
using LoboLabs.Utilities;
using System.Collections.Generic;
using System;

namespace LoboLabs.GestureNeuralNet
{

    public class GestureScape : Scape
    {
        private static ClassLogger Logger = new ClassLogger(typeof(GestureScape));

        private List<Vector> mCurrentData;
        private int mNumPoints;

        public GestureScape(int numPoints)
        {
            mNumPoints = numPoints;
            mCurrentData = new List<Vector>();

            if (numPoints < 4)
            {
                throw new NotImplementedException();
            }
        }

        public virtual void StartGesturing()
        {
            mCurrentData.Clear();
        }

        public virtual void StopGesturing()
        {
            List<double> convertedData = new List<double>();
            
            // TODO: Can this be more efficient?
            // Estimate the gesture down to the set number of points + 1 in order to 
            // result in 'points' number of direction vectors
            while (mCurrentData.Count > mNumPoints + 1)
            {
                List<Vector> estimatedGesture = new List<Vector>(mCurrentData.Count - 1);

                // Add first location to preserve start location
                estimatedGesture.Add(mCurrentData[0]);

                // Midpoint estimation
                for(int datum = 1; datum < mCurrentData.Count - 2; ++datum)
                {
                    Vector midpoint = ((mCurrentData[datum + 1] - mCurrentData[datum]) / 2) + mCurrentData[datum];
                    estimatedGesture.Add(midpoint);
                }

                // Add last location to preserve end location
                estimatedGesture.Add(mCurrentData[mCurrentData.Count - 1]);

                mCurrentData = estimatedGesture;
            }

            // Convert to directional vectors
            for (int datum = 0; datum < mCurrentData.Count - 1; ++datum)
            {
                Vector difference = (mCurrentData[datum + 1] - mCurrentData[datum]).Normalized();
                convertedData.AddRange(difference.ToList());
            }

            NotifyDataReceived(convertedData);
        }

        public virtual void UpdateGesturePosition(Vector position)
        {
            mCurrentData.Add(position);
        }
    }

}
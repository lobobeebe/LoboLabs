using System;
using System.Collections.Generic;

namespace LoboLabs.GestureNeuralNet.Test.Utilities
{
    using LoboLabs.Utilities;

    public class GestureDataGenerator
    {
        public static List<Vector> CreateStraightData(int numPoints)
        {
            List<Vector> gestureData = new List<Vector>();
            gestureData.Add(new Vector(0, 0, 0));

            const float VARIANCE = .3f;
            float currentPosition = 0;

            for (int i = 0; i < numPoints - 1; ++i)
            {
                currentPosition += (float)MathUtils.NextRand(1 - VARIANCE, 1 + VARIANCE);
                gestureData.Add(new Vector(0, 0, currentPosition));
            }

            return gestureData;
        }

        public static List<Vector> CreateSwipeData(int numPoints)
        {
            List<Vector> gestureData = new List<Vector>();
            gestureData.Add(new Vector(0, 0, 0));

            const float VARIANCE = .3f;
            float currentPosition = 0;

            for (int i = 0; i < numPoints - 1; ++i)
            {
                currentPosition += (float)MathUtils.NextRand(1 - VARIANCE, 1 + VARIANCE);
                gestureData.Add(new Vector(0, currentPosition, 0));
            }

            return gestureData;
        }

        public static List<Vector> CreateCircleData(float radius, int numPoints, bool clockwise = true)
        {
            List<Vector> gestureData = new List<Vector>();
            gestureData.Add(new Vector(0, 0, 0));

            double angleOffset = 2 * Math.PI / (numPoints - 1);
            if (!clockwise)
            {
                angleOffset *= -1;
            }

            for (double angle = 0; Math.Abs(angle) < 2 * Math.PI; angle += angleOffset)
            {
                Vector position = new Vector((float)System.Math.Cos(angle) * radius, (float)System.Math.Sin(angle) * radius, 0);
                gestureData.Add(position);
            }

            return gestureData;
        }

        public static List<Vector> CreateRandomData(float radius, int numPoints)
        {
            List<Vector> gestureData = new List<Vector>();
            gestureData.Add(new Vector(0, 0, 0));

            for (int i = 0; i < numPoints - 1; ++i)
            {
                Vector position = new Vector((float)MathUtils.NextRand(-radius, radius),
                    (float)MathUtils.NextRand(-radius, radius), (float)MathUtils.NextRand(-radius, radius));
                gestureData.Add(position);
            }

            return gestureData;
        }
    }
}

using System.Collections.Generic;

namespace LoboLabs.Utilities
{

    public struct Vector
    {
        public float x;
        public float y;
        public float z;

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public List<double> ToList()
        {
            return new List<double>() { x, y, z };
        }

        public static Vector operator-(Vector subtracted, Vector subtractor)
        {
            return new Vector(subtracted.x - subtractor.x, subtracted.y - subtractor.y, subtracted.z - subtractor.z);
        }
    }
    
}
using System.Collections.Generic;

namespace LoboLabs.Utilities
{

    public struct Vector : NetworkInputType
    {
        private static ClassLogger Logger = new ClassLogger(typeof(Vector));

        public float X;
        public float Y;
        public float Z;

        public Vector(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public static Vector FromList(List<double> List)
        {
            // Lists must have exactly 3 entries to be a valid Vector
            if(List.Count == 3)
            {
                return new Vector((float)List[0], (float)List[1], (float)List[2]);
            }

            Logger.Error("Cannot convert a list of size " + List.Count + " into a Vector. Returning zero vector.");
            return new Vector();
        }

        public List<double> ToList()
        {
            return new List<double>() { X, Y, Z };
        }

        public static Vector operator-(Vector subtracted, Vector subtractor)
        {
            return new Vector(subtracted.X - subtractor.X, subtracted.Y - subtractor.Y, subtracted.Z - subtractor.Z);
        }
    }
    
}
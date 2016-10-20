using System;
using System.Collections.Generic;

namespace LoboLabs.Utilities
{

    public struct Vector
    {
        private static ClassLogger Logger = new ClassLogger(typeof(Vector));

        public float X;
        public float Y;
        public float Z;

        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
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

        public float GetMagnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector Normalized()
        {
            double magnitude = GetMagnitude();
            return new Vector((float)(X / magnitude), (float)(Y / magnitude), (float)(Z / magnitude));
        }

        public List<double> ToList()
        {
            return new List<double>() { X, Y, Z };
        }

        public static Vector operator-(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static Vector operator +(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        public static Vector operator /(Vector lhs, float rhs)
        {
            return new Vector(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs);
        }

        public static Vector operator *(Vector lhs, float rhs)
        {
            return new Vector(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
        }
    }
    
}
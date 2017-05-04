using LoboLabs.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace LoboLabs.NeuralNet
{
    public class Node
    {
        private static List<Node> sNodes = new List<Node>(); 

        public Node(bool isSumming = false)
        {
            IsSumming = isSumming;
            UUID = MathUtils.NextRandUInt();

            sNodes.Add(this);
        }

        public Node(BinaryReader reader)
        {
            Load(reader);
        }

        public void AddErrorSignal(double errorSignal)
        {
            if (IsSumming)
            {
                ErrorSignalSum += errorSignal;
            }
        }

        public bool Equals(Node other)
        {
            return UUID == other.UUID;
        }

        private double ErrorSignalSum
        {
            get;
            set;
        }

        public double GetAndResetErrorSignalSum()
        {
            double errorSignalSum = ErrorSignalSum;
            ErrorSignalSum = 0;

            return errorSignalSum;
        }

        private bool IsSumming
        {
            get;
            set;
        }

        /// <summary>
        /// Stores the last output of this Neuron
        /// </summary>
        public double LastOutput
        {
            get;
            set;
        }

        protected virtual void Load(BinaryReader reader)
        {
            // Read the UUID
            UUID = reader.ReadUInt32();

            // Read the IsSumming bool
            IsSumming = reader.ReadBoolean();
        }

        public virtual void Save(BinaryWriter writer)
        {
            // Write the UUID
            writer.Write(UUID);

            // Write the IsSumming bool
            writer.Write(IsSumming);
        }

        public uint UUID
        {
            get;
            set;
        }
    }

}

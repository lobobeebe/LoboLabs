using System;
using System.Collections.Generic;
using System.IO;
using LoboLabs.NeuralNet;

namespace LoboLabs.GestureNeuralNet
{
    public class GestureDefinition : ScapeDataDefinition
    {
        public GestureDefinition(int numInputs) : base(numInputs)
        {
        }

        /// <summary>
        /// Saves the Gesture Data to a file.
        /// </summary>
        /// <returns></returns>
        public void SaveToFile(string fileName)
        {
            // Open the file
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                // Write the name of the Gesture
                writer.Write(Name);

                // Write the number of positions per gesture
                writer.Write(NumInputs);

                // Write the length of the Data list
                writer.Write(DataList.Count);

                // Write each x, y, z of the positions
                foreach (ScapeData data in DataList)
                {
                    data.WriteToStream(writer);
                }
            }
        }

        /// <summary>
        /// Loads the Gesture Data from a file.
        /// </summary>
        /// <returns></returns>
        public void LoadFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                // Clear the current gesture
                DataList.Clear();

                // Open the file
                using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    // Read the name of the Gesture
                    Name = reader.ReadString();

                    // Read the number of positions per gesture
                    NumInputs = reader.Read();

                    // Read the length of the Positions vector
                    int numGestureData = reader.Read();

                    // Read each x, y, z of the positions
                    for (int i = 0; i < numGestureData; ++i)
                    {
                        GestureData data = new GestureData(NumInputs);
                        data.LoadFromStream(reader);

                        AddScapeData(data);
                    }
                }
            }
        }
    }
}

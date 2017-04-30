using System.Collections.Generic;
using System.IO;

namespace LoboLabs.NeuralNet
{
    public class DataClass
    {
        public DataClass(string name = "", uint numInputs = 0)
        {
            Name = name;
            NumInputs = numInputs;

            DataList = new List<ScapeData>();
        }

        public void AddScapeData(ScapeData data)
        {
            DataList.Add(data);
        }
        
        public List<ScapeData> DataList
        {
            get;
            private set;
        }

        public static List<string> GetNamesFromList(List<DataClass> definitionList)
        {
            List<string> nameList = new List<string>();

            foreach(DataClass definition in definitionList)
            {
                nameList.Add(definition.Name);
            }

            return nameList;
        }

        public string Name
        {
            get;
            protected set;
        }

        public uint NumInputs
        {
            get;
            protected set;
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
                // Open the file
                using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    // Read the name of the Gesture
                    Name = reader.ReadString();

                    // Read the number of positions per gesture
                    NumInputs = reader.ReadUInt32();
                    
                    // Read the length of the Positions vector
                    int numGestureData = reader.ReadInt32();

                    // Read each x, y, z of the positions
                    for (int i = 0; i < numGestureData; ++i)
                    {
                        ScapeData data = LoadDataFromStream(reader);

                        AddScapeData(data);
                    }
                }
            }
        }

        protected virtual ScapeData LoadDataFromStream(BinaryReader reader)
        {
            return null;
        } 
    }
}

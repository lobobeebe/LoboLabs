using System.Collections.Generic;
using LoboLabs.NeuralNet;
using System.IO;

namespace LoboLabs.GestureNeuralNet
{
    /// <summary>
    /// Allows the User to save and load Gesture Definitions from disk
    /// </summary>
    public class GestureManager
    {
        private static string GESTURE_EXTENSION = ".gd";

        public static List<DataClass> GetGesturesFromPath(string directory)
        {
            List<DataClass> definitionList = new List<DataClass>();

            if (Directory.Exists(directory))
            {
                string[] filesInDirectory = Directory.GetFiles(directory, "*" + GESTURE_EXTENSION);

                foreach (string file in filesInDirectory)
                {
                    GestureClass definition = new GestureClass();
                    definition.LoadFromFile(file);

                    if (definition != null)
                    {
                        definitionList.Add(definition);
                    }
                }
            }
            
            return definitionList;
        }

        public static List<DataClass> SaveGesturesToPath(string directory, List<DataClass> definitionList)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            foreach (DataClass definition in definitionList)
            {
                definition.SaveToFile(directory + "\\" + definition.Name + GESTURE_EXTENSION);
            }

            return definitionList;
        }
    }
}

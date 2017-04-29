using System;
using System.Collections.Generic;
using LoboLabs.NeuralNet;
using LoboLabs.Utilities;
using System.IO;

namespace LoboLabs.GestureNeuralNet
{
    /// <summary>
    /// Allows the User to save and load Gesture Definitions from disk
    /// </summary>
    public class GestureIOManager
    {
        private static string GESTURE_EXTENSION = ".gd";

        public static List<ScapeDataDefinition> GetGesturesFromPath(string directory)
        {
            List<ScapeDataDefinition> definitionList = new List<ScapeDataDefinition>();

            if (Directory.Exists(directory))
            {
                string[] filesInDirectory = Directory.GetFiles(directory, "*" + GESTURE_EXTENSION);

                foreach (string file in filesInDirectory)
                {
                    ScapeDataDefinition definition = new ScapeDataDefinition("", 0);
                    definition.LoadFromFile(file);

                    if (definition != null)
                    {
                        definitionList.Add(definition);
                    }
                }
            }
            
            return definitionList;
        }

        public static List<ScapeDataDefinition> SaveGesturesToPath(string directory, List<ScapeDataDefinition> definitionList)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            foreach (ScapeDataDefinition definition in definitionList)
            {
                definition.SaveToFile(directory + "\\" + definition.Name + GESTURE_EXTENSION);
            }

            return definitionList;
        }
    }
}

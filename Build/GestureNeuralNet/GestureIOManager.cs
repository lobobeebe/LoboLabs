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

        public static List<GestureDefinition> GetGesturesFromPath(string directory)
        {
            List<GestureDefinition> definitionList = new List<GestureDefinition>();

            if (Directory.Exists(directory))
            {
                string[] filesInDirectory = Directory.GetFiles(directory, "*" + GESTURE_EXTENSION);

                foreach (string file in filesInDirectory)
                {
                    GestureDefinition definition = GestureDefinition.LoadFromFile(file);

                    if (definition != null)
                    {
                        definitionList.Add(definition);
                    }
                }
            }

            return definitionList;
        }

        public static List<GestureDefinition> SaveGesturesToPath(string directory, List<GestureDefinition> definitionList)
        {
            if (Directory.Exists(directory))
            {
                foreach (GestureDefinition definition in definitionList)
                {
                    definition.SaveToFile(directory + definition.Name + GESTURE_EXTENSION);
                }
            }

            return definitionList;
        }
    }
}

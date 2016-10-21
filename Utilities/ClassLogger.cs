using System;

namespace LoboLabs.Utilities
{
    /// <summary>
    /// TODO
    /// </summary>
    public class ClassLogger
    {
        public ClassLogger(Type type)
        {
            ClassType = type;
        }

        private Type ClassType
        {
            get;
            set;
        }

        public void Debug(string message)
        {
            Log("Debug", message);
        }

        public void Error(string message)
        {
            Log("Error", message);
        }

        private void Log(string type, string message)
        {
            LogWriter.Write("[" + ClassType.Name + " - " + type + "]" + message);
        }

        public void Warn(string message)
        {
            Log("Warn", message);
        }
    }
}
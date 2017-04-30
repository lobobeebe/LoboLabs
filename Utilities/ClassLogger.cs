using System;

namespace LoboLabs.Utilities
{
    /// <summary>
    /// Logger used to print out messages to a log
    /// </summary>
    public class ClassLogger
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Class type. Name of the Type is used to print out messages.</param>
        public ClassLogger(Type type)
        {
            ClassType = type;
        }
    
        /// <summary>
        /// The Class Type used to create the Class Logger
        /// </summary>
        public Type ClassType
        {
            get;
            set;
        }
    
        /// <summary>
        /// Used to print Debug messages
        /// </summary>
        /// <param name="message">The debug message to print</param>
        public void Debug(string message)
        {
            Log("Debug", message);
        }

        /// <summary>
        /// Used to print Error messages
        /// </summary>
        /// <param name="message">The error message to print</param>
        public void Error(string message)
        {
            Log("Error", message);
        }

        /// <summary>
        /// Used to print messages
        /// </summary>
        /// <param name="type">Type of the message to print</param>
        /// <param name="message">Message to print</param>
        private void Log(string type, string message)
        {
            LogWriter.Write("[" + ClassType.Name + " - " + type + "]" + message);
        }
    }
}
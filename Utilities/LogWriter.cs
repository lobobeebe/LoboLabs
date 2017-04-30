using System;

namespace LoboLabs.Utilities
{
    /// <summary>
    /// Class used to log messages
    /// </summary>
    public class LogWriter
    {
        private static LogWriter mSingletonLogWriter;

        /// <summary>
        /// Static function used to write messages
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
        {
            SingletonLogWriter._Write(message);
        }

        protected virtual void _Write(string message)
        {
            Console.WriteLine(message);
        }

        public static void SetLogWriter(LogWriter logWriter)
        {
            SingletonLogWriter = logWriter;
        }

        private static LogWriter SingletonLogWriter
        {
            get
            {
                if (mSingletonLogWriter == null)
                {
                    mSingletonLogWriter = new LogWriter();
                }

                return mSingletonLogWriter;
            }
            set
            {
                mSingletonLogWriter = value;
            }
        }
    }
}
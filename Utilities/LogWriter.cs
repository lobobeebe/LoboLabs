using System;

namespace LoboLabs
{ 
namespace Utilities
{

public class LogWriter
{
    public static void Write(string message)
    {
        if(SingletonLogWriter == null)
        {
            SingletonLogWriter = new LogWriter();
        }

        SingletonLogWriter.WriteMessage(message);
    }

    protected virtual void WriteMessage(string message)
    {
        Console.WriteLine(message);
    }

    public static void SetLogWriter(LogWriter logWriter)
    {
        SingletonLogWriter = logWriter;
    }

    private static LogWriter SingletonLogWriter
    {
        get;
        set;
    }
}

}
}
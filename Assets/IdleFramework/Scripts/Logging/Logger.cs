using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger
{
    public static Level globalLevel = Level.ERROR;

    private void Log(Level level, object toLog)
    {
        if (level < globalLevel)
        {
            if (level == Level.WARN)
            {
                UnityEngine.Debug.LogWarning(toLog);
            } else if (level == Level.ERROR)
            {
                UnityEngine.Debug.LogError(toLog);
            } else
            {
                UnityEngine.Debug.Log(toLog);
            }
        } 
    }

    public void Warning(object toLog)
    {
        Log(Level.WARN, toLog);
    }

    public void Error(object toLog)
    {
        Log(Level.ERROR, toLog);
    }

    public void Info(object toLog)
    {
        Log(Level.INFO, toLog);
    }

    public void Debug(object toLog)
    {
        Log(Level.DEBUG, toLog);
    }

    public void Trace(object toLog)
    {
        Log(Level.TRACE, toLog);
    }

    public static Logger GetLogger()
    {
        return new Logger();
    }

    public enum Level
    {
        ERROR,
        WARN,
        INFO,
        DEBUG,
        TRACE
    }
}

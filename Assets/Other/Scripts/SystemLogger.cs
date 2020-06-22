using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
//Logs the system messages
public static class SystemLogger
{
    private static string logFileName;//The file name that we initialized
    private static List<string> messages;//The messages that the system chat saved
    //Init the chat logger
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void StartLogger()
    {
        logFileName = "chatlog_" + DateTime.Now.ToString().Replace(':', '_');
        if (!Directory.Exists(Application.persistentDataPath + "/logs/")) Directory.CreateDirectory(Application.persistentDataPath + "/logs/");        
        messages = new List<string>();
    }
    //Logs a new message to the txt file with a specified timestamp
    public static void LogNewMessage(string message)
    {
        messages.Add(DateTime.Now.ToString() + ": " + message);//Add the new message
        SaverLoader.SaveTxtLines("logs/" + logFileName + ".txt", messages.ToArray());
    }    
}

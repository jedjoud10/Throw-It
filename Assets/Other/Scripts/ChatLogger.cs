using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
//Logs the system chat messages
public static class ChatLogger
{
    private static string logFileName;//The file name that we initialized
    private static List<string> chatMessages;//The messages that the system chat saved
    //Init the chat logger
    public static void StartLogger()
    {
        logFileName = "chatlog_" + DateTime.Now.ToString().Replace(':', '_');
        if (!Directory.Exists(Application.persistentDataPath + "/logs/")) Directory.CreateDirectory(Application.persistentDataPath + "/logs/");        
        chatMessages = new List<string>();
        Debug.Log(logFileName);
    }
    //Logs a new message to the txt file with a specified timestamp
    public static void LogNewMessage(string message, DateTime time)
    {
        chatMessages.Add(time.ToString() + ": " + message);//Add the new message
        SaverLoader.SaveTxtLines("logs/" + logFileName + ".txt", chatMessages.ToArray());
    }
}

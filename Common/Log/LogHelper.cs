using System;
using System.IO;

namespace FaceRecognition.Common.Log;

/**
 * 日志帮助类
 */
public static class LogHelper
{
    public static string LogFilePath = "";

    public static void WriteLog(string message)
    {
        using (StreamWriter streamWriter = new StreamWriter(LogFilePath))
        {
            streamWriter.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}
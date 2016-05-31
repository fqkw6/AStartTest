using System;
using UnityEngine;
using System.Collections;
using System.IO;

// 游戏log记录类，统一使用这个来记录log
// 编译为dll，防止跳转时跳到此源码里面
public class Log
{
    static Log()
    {
        EnableOutput(LogOutput.DebugConsole, true);
    }

    // log输出到什么地方
    public enum LogOutput
    {
        DebugConsole = 1,       // 编辑器的输出console
        GUIConsole = 2,         // 游戏中可显示的console
        File = 4,               // 输出到文件
    }

    // log的输出等级
    public enum LogLevel
    {
        INFO,
        WARNING,
        ERROR,
    }

    private static int _logOutputFlag = 0;
    private static LogLevel _level = LogLevel.INFO;

    // 输出到文件
    private static string _logPath = null;
    private static string _logFileName = "log_{0}.txt";
    private static FileStream _fs;
    private static StreamWriter _sw;

    // 开启输出
    public static void EnableOutput(LogOutput output, bool value)
    {
        if (value) {
            _logOutputFlag |= (int)output;
        } else {
            _logOutputFlag ^= (int)output;
        }

        if ((_logOutputFlag & (int)LogOutput.File) != 0) {
            if (_logPath == null) {
                _logPath = Application.persistentDataPath + "/log/";
            }
            if (!Directory.Exists(_logPath)) {
                Directory.CreateDirectory(_logPath);
            }

            string filePath = _logPath + string.Format(_logFileName, DateTime.Today.ToString("yyyyMMdd"));
            try {
                _fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                _sw = new StreamWriter(_fs);
            } catch (Exception ex) {
                Exception(ex);
            }
        }
    }

    // 设置log的输出等级
    public static void SetLogLevel(LogLevel level)
    {
        _level = level;
    }

    private static void WriteToFile(string text)
    {
        if (_sw == null) {
            return;
        }

        try {
            _sw.WriteLine(text);
            _sw.Flush();
        } catch (Exception e) {
            _sw = null;
            Exception(e);
        }
    }

    public static void Info(object obj)
    {
        if (_level > LogLevel.INFO) {
            return;
        }

        if ((_logOutputFlag & (int)LogOutput.DebugConsole) != 0) {
            UnityEngine.Debug.Log(obj);
        }

        if ((_logOutputFlag & (int)LogOutput.GUIConsole) != 0) {
            //KGFDebug.LogDebug(obj.ToString());
        }

        if ((_logOutputFlag & (int)LogOutput.File) != 0) {
            WriteToFile(obj.ToString());
        }
    }

    public static void Info(string fmt, params object[] param)
    {
        if (_level > LogLevel.INFO) {
            return;
        }

        string text = string.Format(fmt, param);
        if ((_logOutputFlag & (int)LogOutput.DebugConsole) != 0) {
            UnityEngine.Debug.Log(text);
        }

        if ((_logOutputFlag & (int)LogOutput.GUIConsole) != 0) {
            //KGFDebug.LogDebug(text);
        }

        if ((_logOutputFlag & (int)LogOutput.File) != 0) {
            WriteToFile(text);
        }
    }

    public static void Warning(object obj)
    {
        if (_level > LogLevel.WARNING) {
            return;
        }

        if ((_logOutputFlag & (int)LogOutput.DebugConsole) != 0) {
            UnityEngine.Debug.Log(obj);
        }

        if ((_logOutputFlag & (int)LogOutput.GUIConsole) != 0) {
            //KGFDebug.LogDebug(obj.ToString());
        }

        if ((_logOutputFlag & (int)LogOutput.File) != 0) {
            WriteToFile(obj.ToString());
        }
    }

    public static void Warning(string fmt, params object[] param)
    {
        if (_level > LogLevel.WARNING) {
            return;
        }

        string text = string.Format(fmt, param);
        if ((_logOutputFlag & (int)LogOutput.DebugConsole) != 0) {
            UnityEngine.Debug.LogWarning(text);
        }

        if ((_logOutputFlag & (int)LogOutput.GUIConsole) != 0) {
            //KGFDebug.LogWarning(text);
        }

        if ((_logOutputFlag & (int)LogOutput.File) != 0) {
            WriteToFile(text);
        }
    }

    public static void Error(object obj)
    {
        if (_level > LogLevel.ERROR) {
            return;
        }

        if ((_logOutputFlag & (int)LogOutput.DebugConsole) != 0) {
            UnityEngine.Debug.Log(obj);
        }

        if ((_logOutputFlag & (int)LogOutput.GUIConsole) != 0) {
            //KGFDebug.LogDebug(obj.ToString());
        }

        if ((_logOutputFlag & (int)LogOutput.File) != 0) {
            WriteToFile(obj.ToString());
        }
    }

    public static void Error(string fmt, params object[] param)
    {
        if (_level > LogLevel.ERROR) {
            return;
        }
        string text = string.Format(fmt, param);
        if ((_logOutputFlag & (int)LogOutput.DebugConsole) != 0) {
            UnityEngine.Debug.LogError(text);
        }

        if ((_logOutputFlag & (int)LogOutput.GUIConsole) != 0) {
            //KGFDebug.LogError(text);
        }

        if ((_logOutputFlag & (int)LogOutput.File) != 0) {
            WriteToFile(text);
        }
    }

    public static void Exception(Exception e)
    {
        if (_level > LogLevel.ERROR) {
            return;
        }

        if ((_logOutputFlag & (int)LogOutput.DebugConsole) != 0) {
            UnityEngine.Debug.LogException(e);
        }

        if ((_logOutputFlag & (int)LogOutput.GUIConsole) != 0) {
            string text = e.Message + "  " + e.Source + "  " + e.StackTrace;
            //KGFDebug.LogError(text);
        }

        if ((_logOutputFlag & (int)LogOutput.File) != 0) {
            string text = e.Message + "  " + e.Source + "  " + e.StackTrace;
            WriteToFile(text);
        }
    }
}

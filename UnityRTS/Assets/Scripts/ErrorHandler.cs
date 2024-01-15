using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


/// <summary>
/// This class handles all exceptions that are created while playing the game and when the game crashes,
/// if you input your email in the crash reporting screen you will get the email sent
/// to the inputted email when the game crashes or when errors occur
///
/// This class also handles all the logging of erros and events to a text file
/// </summary>
public class ErrorHandler : MonoBehaviour
{

    public static ErrorHandler instance;

    public string receivingEmail;

    private void Awake()
    {
        // Implementing the Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        AppDomain.CurrentDomain.UnhandledException += HandleException;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        AppDomain.CurrentDomain.UnhandledException -= HandleException;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Log all events to a general log file
        LogToFile(logString, stackTrace, "GeneralLog.txt");

        // Categorize logs based on log type and log to dedicated files
        switch (type)
        {
            case LogType.Log:
                LogToFile(logString, stackTrace, "InfoLog.txt");
                break;
            case LogType.Warning:
                LogToFile(logString, stackTrace, "WarningLog.txt");
                break;
            case LogType.Error:
            case LogType.Exception:
                // Log errors and exceptions to a dedicated error log file
                LogToFile(logString, stackTrace, "ErrorLog.txt");

                // Optionally, send error emails (you can call SendCrashReportEmail here if needed)
                break;
        }
    }

    private void LogToFile(string logString, string stackTrace, string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName;

        try
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n{logString}\n\nStackTrace:\n{stackTrace}\n\n";
            File.AppendAllText(filePath, logEntry);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error writing log to file ({fileName}): {ex.Message}");
        }
    }


    private void HandleException(object sender, UnhandledExceptionEventArgs args)
    {
        Exception e = (Exception)args.ExceptionObject;
        string crashReport = $"Exception: {e.Message}\n\nStackTrace:\n{e.StackTrace}";

        // Log the crash report to a file
        WriteCrashReportToFile(crashReport);

        // Optionally, you can call a method to send the crash report via email
        SendCrashReportEmail(crashReport);
    }

    private void WriteCrashReportToFile(string crashReport)
    {
        // Specify the file path where you want to save the crash report
        string filePath = Application.persistentDataPath + "/CrashReport.txt";

        try
        {
            // Write crash report to a text file
            File.WriteAllText(filePath, crashReport);
            Debug.Log($"Crash report saved to: {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error writing crash report to file: {ex.Message}");
        }
    }

    public void TestCrashReport(string crashReport)
    {
        SendCrashReportEmail(crashReport);
    }

    private void SendCrashReportEmail(string crashReport)
    {
        // Probably not safe to hard code my password in but since I am not selling this game I think its alright
        string senderEmail = "jonathan.krolak@gmail.com";
        string recipientEmail = receivingEmail;
        Debug.Log(recipientEmail);
        string password = "dxlm tlkw cdik qvjy"; // Password

        // You can customize the email subject and body
        string subject = "Crash Report";
        string body = crashReport;

        // Attach crash report file (optional)
        string filePath = Application.persistentDataPath + $"/CrashReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

        try
        {
            // Write crash report to a text file
            File.WriteAllText(filePath, crashReport);
            Debug.Log($"Crash report saved to: {filePath}");

            // Send email using UnityGMail class

            UnityGMail.SendMailFromGoogle(senderEmail, recipientEmail, password, subject, body, filePath);
            
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending crash report via email: {ex.Message}");
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class RecordCSVWriter
{
    static string filePath;
    public static void CSV_SetFileName(string fileName)
    {
        if(fileName == null)
        {
            fileName = "test";
        }
        string fileFullName = fileName + ".csv";
        filePath = Path.Combine(Application.persistentDataPath, fileFullName);

        Debug.Log("[RecordCSVWriter] File PATH: " + filePath);
    }

    public static void CSV_Write(string m_case, string extraData = null)
    {
        TextWriter tw = new StreamWriter(filePath, true);
        string content = m_case + "," + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "," + extraData;
        tw.WriteLine(content);
        tw.Close();
    }
    public static void CSV_Write(string m_case, bool realityTimeLog = true, bool gameTimeLog = false, string extraData = null)
    {
        TextWriter tw = new StreamWriter(filePath, true);
        string rtime = realityTimeLog? System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+"," : null;
        string gtime = realityTimeLog? Time.time.ToString()+"," : null;
        string content = m_case + "," + rtime + gtime + extraData;
        tw.WriteLine(content);
        tw.Close();
        Debug.Log("Recorded: " + content);
    }

}

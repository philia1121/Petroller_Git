using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVWriter_Config : MonoBehaviour
{
    public string fileName;
    void Awake()
    {
        if(fileName == null)
        {
            fileName = "default";
        }
        RecordCSVWriter.CSV_SetFileName(fileName);
    }  
}

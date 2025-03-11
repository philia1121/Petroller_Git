using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoBoard : MonoBehaviour
{
    public TextMeshProUGUI TimeText;
    void Update()
    {
        TimeText.text = Time.time.ToString("0.00");
    }
}

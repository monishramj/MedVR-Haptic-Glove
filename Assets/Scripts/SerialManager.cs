using UnityEngine;
using System;
using System.IO.Ports;

public class SerialManager : MonoBehaviour
{
    SerialPort dataStream = new SerialPort("COM11", 115200);
    public string potentiometerInput;

    [SerializeField]
    private FingerBending fingers;

    void Start()
    {
        try
        {
            // Sets DTR and RTS so Unity can comm w/ ESP
            dataStream.DtrEnable = true;  
            dataStream.RtsEnable = true;
            dataStream.ReadTimeout = 1000;
            dataStream.Open();
            Debug.Log("Serial port opened."); 
        }
        catch (System.Exception e)
        {
            // Sometimes other programs checking port
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
    }

    void Update()
    {
        if (dataStream.IsOpen)
        {
            try
            {
                potentiometerInput = dataStream.ReadLine(); 
                //Debug.Log("Potentiometer Data: " + potentiometerInput);
                
                if (potentiometerInput.StartsWith("POT:"))
                {
                    string trimmedInput = potentiometerInput.Substring(4);
                    string[] curlData = trimmedInput.Split(",");
                    int[] newCurls = Array.ConvertAll(curlData, int.Parse);

                    if (fingers.thumbCurl != newCurls[0])
                        fingers.thumbCurl = newCurls[0];

                    if (fingers.pointerCurl != newCurls[1])
                        fingers.pointerCurl = newCurls[1];

                    if (fingers.middleCurl != newCurls[2])
                        fingers.middleCurl = newCurls[2];

                    if (fingers.ringCurl != newCurls[3])
                        fingers.ringCurl = newCurls[3];

                    if (fingers.pinkyCurl != newCurls[4])
                        fingers.pinkyCurl = newCurls[4];
                }
            }
            catch (System.Exception e)
            {
                // Errors might arise with no data transmission
                Debug.LogWarning("Serial read error: " + e.Message);
            }
        }
        else {
            // Nothing is sending altogether
            Debug.LogWarning("It's zero yo.");
        }

    }

    void OnApplicationQuit()
    {
        // Close dataStream to not let it run
        if (dataStream != null && dataStream.IsOpen)
        {
            dataStream.Close();
        }
    }

    public void SendToArduino(string message) {
        if (dataStream != null && dataStream.IsOpen) {
            try {
                dataStream.Write(message);
                Debug.Log("SENT " + "\"" + message + "\"" + " to Arduino"); 
            }
            catch (Exception e) {
                Debug.LogError("Failed Arduino Send: " + e.Message);
            }
        }
    }
}
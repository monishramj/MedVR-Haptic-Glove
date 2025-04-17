using UnityEngine;
using System.IO.Ports;

public class PotentiometerReading : MonoBehaviour
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
                // Updates input every frame
                potentiometerInput = dataStream.ReadLine(); 
                Debug.Log("Potentiometer Data: " + potentiometerInput);
                
                // Assigning each data value to FingerBending.cs
                string[] curlData = potentiometerInput.Split(",");
                int thumbCurl = int.Parse(curlData[0]);
                int pointerCurl = int.Parse(curlData[1]);
                int middleCurl = int.Parse(curlData[2]);
                int ringCurl = int.Parse(curlData[3]);
                int pinkyCurl = int.Parse(curlData[4]);

                if (fingers.thumbCurl != thumbCurl)
                    fingers.thumbCurl = thumbCurl;

                if (fingers.pointerCurl != pointerCurl)
                    fingers.pointerCurl = pointerCurl;

                if (fingers.middleCurl != middleCurl)
                    fingers.middleCurl = middleCurl;

                if (fingers.ringCurl != ringCurl)
                    fingers.ringCurl = ringCurl;

                if (fingers.pinkyCurl != pinkyCurl)
                    fingers.pinkyCurl = pinkyCurl;
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
}